using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Application.Services.Services.BotServices;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.CallbackQueryCommands;

public class StartLaterQueryCommand : ICallbackQueryCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, CallbackQuery query,
        ServiceFacade serviceFacade)
    {
        data!.State = State.SetDate;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Через сколько вы хотите начать работу? В формате: <code>[чч:мм:сс] или [Д.чч:мм:сс]</code>",
            ParseMode.Html,
            replyMarkup: MainKeyboard.Main);
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data)
    {
        return query.Data == "startLater" && data!.State == State.SelectTimeMode;
    }
}