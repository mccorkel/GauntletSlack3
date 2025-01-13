namespace GauntletSlack3.Api.Services.Interfaces
{
    public interface IMessageQueueService
    {
        Task ProcessPendingMessagesAsync();
    }
} 