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
    public class RabbitSubscriber : IStartable, IStopable, IMessageDeserializer<byte[]>
    {
        private readonly ILog _log;
        private readonly IBlobSaver _blobSaver;
        private readonly string _connectionString;
        private readonly string _exchangeName;
        private RabbitMqSubscriber<byte[]> _subscriber;

        public RabbitSubscriber(
            ILog log,
            IBlobSaver blobSaver,
            IShutdownManager shutdownManager,
            string connectionString,
            string exchangeName)
        {
            _log = log;
            _blobSaver = blobSaver;
            _connectionString = connectionString;
            _exchangeName = exchangeName;

            shutdownManager.Register(this, 0);
        }

        public void Start()
        {
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

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }

        public byte[] Deserialize(byte[] data)
        {
            return data;
        }
    }
}
