using Telegram.Bot.Types;

namespace LikeBotVK.Application.Abstractions.Services.BotServices;

public interface IUpdateHandler
{
    Task HandleAsync(Update update);
}