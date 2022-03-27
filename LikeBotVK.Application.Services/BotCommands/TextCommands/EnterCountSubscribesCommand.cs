using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Abstractions.Exceptions;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.TextCommands;

public class EnterCountSubscribesCommand : ITextCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, Message message,
        ServiceFacade serviceFacade)
    {
        if (!int.TryParse(message.Text, out var count))
        {
            await client.SendTextMessageAsync(message.From!.Id, "Введите число!", replyMarkup: MainKeyboard.Main);
            return;
        }

        if (count > 100)
        {
            await client.SendTextMessageAsync(message.From!.Id, "Слишком большое количество!",
                replyMarkup: MainKeyboard.Main);
            return;
        }

        var cost = count * serviceFacade.Configuration.SubscribeCost;

        var bonus = data!.BonusAccount > cost / 2 ? cost / 2 : data.BonusAccount;

        PaymentData payment;
        try
        {
            payment = await serviceFacade.PaymentService.CreatePayAsync(user!.Id, cost - bonus);
        }
        catch (ErrorCreateBillException)
        {
            await client.SendTextMessageAsync(message.From!.Id, "Не удалось выставить счёт. Попробуйте ещё раз.",
                replyMarkup: MainKeyboard.Main);
            return;
        }

        data.State = State.Main;
        data.BonusAccount -= bonus;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
        await client.SendTextMessageAsync(message.From!.Id,
            $"💸 Оплата подписок на сумму {cost}₽ из которых {bonus}₽ из бонусного счёта.\n📆 Дата: {DateTime.Now:dd.MMM.yyyy}\n❌ Статус: Не оплачено.\n\n💳 Оплатите счет.",
            replyMarkup: PaymentKeyboard.CheckBill(payment, count));
    }

    public bool Compare(Message message, User? user, UserData? data) =>
        message.Type == MessageType.Text && data!.State == State.EnterCountToBuy;
}