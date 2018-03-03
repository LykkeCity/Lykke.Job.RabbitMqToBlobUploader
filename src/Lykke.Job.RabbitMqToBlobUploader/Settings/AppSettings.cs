using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.RabbitMqToBlobUploader.Settings
{
    public class AppSettings
    {
        public RabbitMqToBlobUploaderSettings RabbitMqToBlobUploaderJob { get; set; }

        public SlackNotificationsSettings SlackNotifications { get; set; }
    }

    public class SlackNotificationsSettings
    {
        public AzureQueuePublicationSettings AzureQueue { get; set; }
    }

    public class AzureQueuePublicationSettings
    {
        public string ConnectionString { get; set; }

        public string QueueName { get; set; }
    }

    public class RabbitMqToBlobUploaderSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }

        [AzureBlobCheck]
        public string BlobConnectionString { get; set; }

        public bool IsPublicContainer { get; set; }

        public string ContainerName { get; set; }

        public bool UseBatchingByHour { get; set; }

        public int MinBatchCount { get; set; }

        public int MaxBatchCount { get; set; }

        public RabbitMqSettings Rabbit { get; set; }
    }

    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }

        public string ExchangeName { get; set; }
    }
}
