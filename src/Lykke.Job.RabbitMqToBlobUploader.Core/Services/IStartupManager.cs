using System.Threading.Tasks;
using Autofac;

namespace Lykke.Job.RabbitMqToBlobUploader.Core.Services
{
    public interface IStartupManager
    {
        Task StartAsync();

        void Register(IStartable startable);
    }
}
