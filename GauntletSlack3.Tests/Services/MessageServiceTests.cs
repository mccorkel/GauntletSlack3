using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using GauntletSlack3.Services;
using GauntletSlack3.Models;

public class MessageServiceTests
{
    private readonly Mock<HttpClient> _httpClientMock;
    private readonly Mock<ILogger<MessageService>> _loggerMock;
    private readonly MessageService _messageService;

    [Fact]
    public async Task SendMessageAsync_ValidMessage_ReturnsSuccess()
    {
        // Arrange
        var message = new Message { Content = "Test", ChannelId = 1, UserId = 1 };

        // Act
        var result = await _messageService.SendMessageAsync(message.ChannelId, message.UserId, message.Content);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(message.Content, result.Content);
    }
} 