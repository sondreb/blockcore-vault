using Blockcore.Vault.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blockcore.Vault.Managers
{
    public class SyncManager : IDisposable
    {
        private readonly ILogger<SyncManager> log;
        private readonly ChainSettings chainSettings;
        private readonly SyncSettings syncSettings;

        private readonly IServiceProvider serviceProvider;

        public SyncManager(
           ILogger<SyncManager> log,
           IOptions<ChainSettings> chainSettings,
           IOptions<SyncSettings> syncSettings,
           IServiceProvider serviceProvider)
        {
            this.log = log;
            this.chainSettings = chainSettings.Value;
            this.syncSettings = syncSettings.Value;
            this.serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (!syncSettings.Enabled)
            {
                log.LogInformation($"Sync Service is disabled.");
            }

            log.LogInformation($"Start Sync Service for {chainSettings.Symbol}.");

            Task.Run(async () =>
            {
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        log.LogInformation("Spin up threads for individual connected vaults.");

                        // TODO: This loop will just continue to run after connected to gateway. It should check status and attempt to recycle and reconnect when needed.
                        Task.Delay(TimeSpan.FromSeconds(10), cancellationToken).Wait(cancellationToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    // do nothing the task was cancel.
                    throw;
                }
                catch (Exception ex)
                {
                    log.LogError(ex, "Sync");
                    throw;
                }

            }, cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {

        }
    }
}
