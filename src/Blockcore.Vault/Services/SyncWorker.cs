using Blockcore.Vault.Managers;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blockcore.Vault.Services
{
    public class SyncWorker : IHostedService
    {
        private readonly SyncManager manager;

        public SyncWorker(SyncManager manager)
        {
            this.manager = manager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return manager.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return manager.StopAsync(cancellationToken);
        }
    }
}
