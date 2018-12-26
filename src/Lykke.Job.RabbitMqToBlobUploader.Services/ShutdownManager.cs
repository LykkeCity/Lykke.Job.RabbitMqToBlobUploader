using Common;
using Lykke.Job.RabbitMqToBlobUploader.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.RabbitMqToBlobUploader.Services
{
    public class ShutdownManager : IShutdownManager
    {
        private readonly IEnumerable<IStopable> _stopables;

        public ShutdownManager(IEnumerable<IStopable> stopables)
        {
            _stopables = stopables;
        }

        public Task StopAsync()
        {
            Parallel.ForEach(_stopables, i => i.Stop());

            return Task.CompletedTask;
        }
    }
}
