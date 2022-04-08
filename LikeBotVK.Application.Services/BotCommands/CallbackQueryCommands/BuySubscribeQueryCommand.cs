using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Application.Services.Services.BotServices;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace LikeBotVK.Application.Services.BotCommands.CallbackQueryCommands;

public class BuySubscribeQueryCommand : ICallbackQueryCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, Domain.Users.Entities.User? user, UserData? data,
        CallbackQuery query,
        ServiceFacade serviceFacade)
    {
        if (data!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            $"Введите количество подписок, которые хотите приобрести. Цена одной подписки - {serviceFacade.Configuration.SubscribeCost} рублей/{serviceFacade.Configuration.SubscribeDuration} дней.",
            replyMarkup: MainKeyboard.Main);
        data.State = State.EnterCountToBuy;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
    }

    public bool Compare(CallbackQuery query, Domain.Users.Entities.User? user, UserData? data) =>
        query.Data == "buySubscribe";
}