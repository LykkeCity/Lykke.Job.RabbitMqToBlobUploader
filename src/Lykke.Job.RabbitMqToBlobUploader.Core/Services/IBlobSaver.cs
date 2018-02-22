using System.Threading.Tasks;
using Autofac;
using Common;

namespace Lykke.Job.RabbitMqToBlobUploader.Core.Services
{
    public interface IBlobSaver : IStartable, IStopable
    {
        Task AddDataItemAsync(byte[] item);
    }
}
