using System.Linq;
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
        private readonly Dictionary<int, List<IStopable>> _items = new Dictionary<int, List<IStopable>>();

        public ShutdownManager(ILog log)
        {
            _log = log;
        }

        public void Register(IStopable stopable, int priority)
        {
            if (_items.ContainsKey(priority))
                _items[priority].Add(stopable);
            else
                _items.Add(priority, new List<IStopable> { stopable });
        }

        public async Task StopAsync()
        {
            foreach (var priority in _items.Keys.OrderBy(i => i))
            {
                foreach (var item in _items[priority])
                {
                    item.Stop();
                }
            }

            await Task.CompletedTask;
        }
    }
}
