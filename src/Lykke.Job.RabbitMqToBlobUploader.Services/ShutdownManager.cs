using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Job.RabbitMqToBlobUploader.Core.Services;

namespace Lykke.Job.RabbitMqToBlobUploader.Services
{
    public class ShutdownManager : IShutdownManager
    {
        private readonly ILog _log;
        private readonly IEnumerable<IStopable> _stopables;

        public ShutdownManager(ILog log, IEnumerable<IStopable> stopables)
        {
            _log = log;
            _stopables = stopables;
        }

        public async Task StopAsync()
        {
            Parallel.ForEach(_stopables, i => i.Stop());

            await Task.CompletedTask;
        }
    }
}
