using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Common.Log;
using Lykke.Job.RabbitMqToBlobUploader.Core.Services;

namespace Lykke.Job.RabbitMqToBlobUploader.Services
{
    public class StartupManager : IStartupManager
    {
        private readonly ILog _log;
        private readonly List<IStartable> _startables = new List<IStartable>();

        public StartupManager(ILog log, IMainProcessor mainProcessor)
        {
            _log = log;
            _startables.Add(mainProcessor);
        }

        public Task StartAsync()
        {
            foreach (var item in _startables)
            {
                item.Start();
            }

            return Task.CompletedTask;
        }
    }
}
