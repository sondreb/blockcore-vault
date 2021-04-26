using Blockcore.Indexer.Storage.Mongo;
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

        private readonly MongoData data;

        private readonly MongoBuilder builder;

        public SyncWorker(SyncManager manager, MongoData data, MongoBuilder builder)
        {
            this.manager = manager;
            this.data = data;
            this.builder = builder;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            builder.Initialize();

            return manager.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return manager.StopAsync(cancellationToken);
        }
    }
}
