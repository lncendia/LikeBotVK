using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.Services.BotServices;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.TextCommands;

public class BanCommand : ITextCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, Message message,
        ServiceFacade serviceFacade)
    {
        await client.SendTextMessageAsync(message.Chat.Id,
            $"Вы были заблокированы. Для обжалования обратитесь в поддержку: {serviceFacade.Configuration.HelpAddress}.");
    }

    public bool Compare(Message message, User? user, UserData? data) => data!.IsBanned;
}