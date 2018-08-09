using Autofac;
using Lykke.Job.RabbitMqToBlobUploader.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.RabbitMqToBlobUploader.Services
{
    public class StartupManager : IStartupManager
    {
        private readonly List<IStartable> _startables = new List<IStartable>();

        public StartupManager(IMainProcessor mainProcessor)
        {
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
