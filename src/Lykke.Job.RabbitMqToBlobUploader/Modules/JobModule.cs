using Autofac;
using Common.Log;
using Lykke.Job.RabbitMqToBlobUploader.Core.Services;
using Lykke.Job.RabbitMqToBlobUploader.Settings;
using Lykke.Job.RabbitMqToBlobUploader.Services;
using Lykke.Job.RabbitMqToBlobUploader.RabbitSubscribers;

namespace Lykke.Job.RabbitMqToBlobUploader.Modules
{
    public class JobModule : Module
    {
        private readonly RabbitMqToBlobUploaderSettings _settings;
        private readonly ILog _log;

        public JobModule(RabbitMqToBlobUploaderSettings settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            builder.RegisterType<BlobSaver>()
                .As<IBlobSaver>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter("blobConnectionString", _settings.BlobConnectionString)
                .WithParameter("container", _settings.ContainerName)
                .WithParameter("isPublicContainer", _settings.IsPublicContainer)
                .WithParameter("useBatchingByHour", _settings.UseBatchingByHour)
                .WithParameter("minBatchCount", _settings.MinBatchCount)
                .WithParameter("maxBatchCount", _settings.MaxBatchCount);

            builder.RegisterType<RabbitSubscriber>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter("connectionString", _settings.Rabbit.ConnectionString)
                .WithParameter("exchangeName", _settings.Rabbit.ExchangeName);
        }
    }
}
