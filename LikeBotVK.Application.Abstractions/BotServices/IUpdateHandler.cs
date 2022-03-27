using Telegram.Bot.Types;

namespace LikeBotVK.Application.Abstractions.BotServices;

public interface IUpdateHandler
{
    Task HandleAsync(Update update);
}