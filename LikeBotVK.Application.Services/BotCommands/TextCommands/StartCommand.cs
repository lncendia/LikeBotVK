using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Application.Services.Services.BotServices;
using Telegram.Bot;
using Telegram.Bot.Types;
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
        await serviceFacade.UnitOfWork.UserRepository.Value.AddAsync(user);
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);

        var t3 = client.SendStickerAsync(message.From.Id,
            new InputOnlineFile("CAACAgIAAxkBAAEEa4diUaDZn6iiE5xQwtU9PHRfwYaxRQAC6RMAAoPgGUtQdqANdCozEiME"),
            replyMarkup: MainKeyboard.MainReplyKeyboard);
        var t4 = client.SendTextMessageAsync(message.Chat.Id,
            "Бот автоматизации для ВКонтакте.\nВыполняет действия от лица Ваших аккаунтов.\n✅Ставит лайки\n✅Делает репосты\n✅Добавляет в друзья");
        await Task.WhenAll(t3, t4);
    }

    public bool Compare(Message message, User? user, UserData? data) => user is null;
}