using System;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Job.RabbitMqToBlobUploader.Core.Services;

namespace Lykke.Job.RabbitMqToBlobUploader.RabbitSubscribers
{
    public class RabbitSubscriber : IStartable, IStopable, IMessageDeserializer<byte[]>, IMainProcessor
    {
        private readonly ILog _log;
        private readonly IBlobSaver _blobSaver;
        private readonly string _connectionString;
        private readonly string _exchangeName;
        private RabbitMqSubscriber<byte[]> _subscriber;

        public RabbitSubscriber(
            ILog log,
            IBlobSaver blobSaver,
            string connectionString,
            string exchangeName)
        {
            _log = log;
            _blobSaver = blobSaver;
            _connectionString = connectionString;
            _exchangeName = exchangeName;
        }

        public void Start()
        {
            _blobSaver.Start();

            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(_connectionString, _exchangeName, "rabbitmqtoblobuploader")
                .MakeDurable();

            _subscriber = new RabbitMqSubscriber<byte[]>(settings,
                    new ResilientErrorHandlingStrategy(_log, settings,
                        retryTimeout: TimeSpan.FromSeconds(10),
                        next: new DeadQueueErrorHandlingStrategy(_log, settings)))
                .SetMessageDeserializer(this)
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .SetLogger(_log)
                .SetConsole(new LogToConsole())
                .Start();
        }

        public void Stop()
        {
            _subscriber?.Stop();

            _blobSaver.Stop();
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        public byte[] Deserialize(byte[] data)
        {
            return data;
        }

        private async Task ProcessMessageAsync(byte[] arg)
        {
            try
            {
                await _blobSaver.AddDataItemAsync(arg);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(RabbitSubscriber), nameof(ProcessMessageAsync), ex);
            }
        }
    }
}
