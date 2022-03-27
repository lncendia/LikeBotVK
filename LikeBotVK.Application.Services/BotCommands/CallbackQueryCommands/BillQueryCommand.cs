using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Exceptions;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Domain.Payments.Entities;
using LikeBotVK.Domain.Users.ValueObjects;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.CallbackQueryCommands;

public class BillQueryCommand : ICallbackQueryCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, CallbackQuery query,
        ServiceFacade serviceFacade)
    {
        var queryData = query.Data!.Split('_', 3);
        Payment payment;
        try
        {
            var (item1, dateTime) = await serviceFacade.PaymentService.GetPaymentData(queryData[2]);
            payment = new Payment(user!.Id, item1, dateTime);
        }
        catch (BillNotPaidException)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Счёт не оплачен.");
            return;
        }
        catch (ErrorCheckBillException)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Произошла ошибка при проверке счёта.");
            return;
        }

        var count = int.Parse(queryData[1]);
        for (var i = 0; i < count; i++)
            user.AddSubscribe(new Subscribe(DateTime.UtcNow.AddDays(serviceFacade.Configuration.SubscribeDuration)));

        await serviceFacade.UnitOfWork.UserRepository.Value.UpdateAsync(user);

        if (data!.ReferralId.HasValue) await DepositBonus(user, data, serviceFacade, client);

        await serviceFacade.UnitOfWork.PaymentRepository.Value.AddAsync(payment);

        var message = query.Message!.Text!;
        message = message.Replace("❌ Статус: Не оплачено", "✔ Статус: Оплачено");
        message = message.Remove(message.IndexOf("Оплачено", StringComparison.Ordinal) + 8);
        await client.EditMessageTextAsync(query.From.Id, query.Message.MessageId, message);
        await client.AnswerCallbackQueryAsync(query.Id, "Успешно оплачено.");
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) => query.Data!.StartsWith("bill");

    private static async Task DepositBonus(User user, UserData data, ServiceFacade serviceFacade,
        ITelegramBotClient client)
    {
        var ownerData =
            await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.GetAsync(data.ReferralId!.Value);
        data.ReferralId = null;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
        if (ownerData != null)
        {
            ownerData.BonusAccount += serviceFacade.Configuration.ReferralBonus;
            await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(ownerData);
            try
            {
                await client.SendTextMessageAsync(ownerData.UserId,
                    $"Ваш реферал {user.Id} впервые пополнил счёт. Вам начислен бонус в размере {serviceFacade.Configuration.ReferralBonus}₽");
            }
            catch
            {
                // ignored
            }
        }
    }
}