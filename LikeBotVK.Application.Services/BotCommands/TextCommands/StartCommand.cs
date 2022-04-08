using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Application.Services.Services.BotServices;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using User = LikeBotVK.Domain.Users.Entities.User;

// ReSharper disable RedundantAssignment

namespace LikeBotVK.Application.Services.BotCommands.TextCommands;

public class StartCommand : ITextCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, Message message,
        ServiceFacade serviceFacade)
    {
        user = new User {Id = message.From!.Id};
        data = new UserData(message.From.Id, State.Main);
        if (message.Text!.Length > 7 && long.TryParse(message.Text[7..], out var id)) data.ReferralId = id;
        var t1 = serviceFacade.UnitOfWork.UserRepository.Value.AddAsync(user);
        var t2 = serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
        await Task.WhenAll(t1, t2);

        var t3 = client.SendStickerAsync(message.From.Id,
            new InputOnlineFile("CAACAgIAAxkBAAEDh2ZhwNXpm0Vikt-5J5yPWTbDPeUwvwAC-BIAAkJOWUoAAXOIe2mqiM0jBA"),
            replyMarkup: MainKeyboard.MainReplyKeyboard);
        var t4 = client.SendTextMessageAsync(message.Chat.Id,
            "<b>Здравствуйте!</b>\n\nDirect рассылка - это бот для рассылки сообщений в директ Аккаунт.\n\nС помощь него Вы можете делать рассылку по хештегу, по подписчикам, по подпискам и по целевой аудитории через файл!",
            ParseMode.Html);
        await Task.WhenAll(t3, t4);
    }

    public bool Compare(Message message, User? user, UserData? data) => user is null;
}