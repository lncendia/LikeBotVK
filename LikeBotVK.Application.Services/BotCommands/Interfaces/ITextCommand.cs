using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Services.Services.BotServices;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.Interfaces;

public interface ITextCommand
{
    public Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, Message message, ServiceFacade serviceFacade);

    public bool Compare(Message message, User? user, UserData? data);
}