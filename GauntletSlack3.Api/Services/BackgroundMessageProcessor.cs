using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using GauntletSlack3.Api.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace GauntletSlack3.Api.Services
{
    public class BackgroundMessageProcessor : BackgroundService
    {
        private readonly ILogger<BackgroundMessageProcessor> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public BackgroundMessageProcessor(
            ILogger<BackgroundMessageProcessor> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var messageQueueService = scope.ServiceProvider.GetRequiredService<IMessageQueueService>();
                        await messageQueueService.ProcessPendingMessagesAsync();
                    }

                    // Wait before processing next batch
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown, log at debug level
                _logger.LogDebug("Background service shutting down normally");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in background message processor");
                // Don't rethrow - let the service continue running
            }
        }
    }
} 