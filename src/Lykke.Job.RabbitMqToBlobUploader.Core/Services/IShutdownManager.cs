using System.Threading.Tasks;

namespace Lykke.Job.RabbitMqToBlobUploader.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}
