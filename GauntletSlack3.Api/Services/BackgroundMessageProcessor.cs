using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using GauntletSlack3.Api.Services.Interfaces;

namespace GauntletSlack3.Api.Services
{
    public class BackgroundMessageProcessor : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BackgroundMessageProcessor> _logger;

        public BackgroundMessageProcessor(
            IServiceScopeFactory scopeFactory,
            ILogger<BackgroundMessageProcessor> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var messageQueueService = scope.ServiceProvider.GetRequiredService<IMessageQueueService>();
                        // Your processing logic here
                        await ProcessMessages(messageQueueService, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing messages");
                }

                await Task.Delay(1000, stoppingToken); // Or whatever delay you want
            }
        }

        private async Task ProcessMessages(IMessageQueueService messageQueueService, CancellationToken stoppingToken)
        {
            // Your existing message processing logic here
        }
    }
} 