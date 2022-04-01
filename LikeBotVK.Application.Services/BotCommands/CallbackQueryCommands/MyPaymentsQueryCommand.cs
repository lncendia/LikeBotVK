using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.CallbackQueryCommands;

public class MyPaymentsQueryCommand : ICallbackQueryCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, CallbackQuery query,
        ServiceFacade serviceFacade)
    {
        if (data!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        var page = int.Parse(query.Data![16..]);
        if (page < 1)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы в конце списка.");
            return;
        }

        var payments =
            await serviceFacade.ApplicationDataUnitOfWork.PaymentDataRepository.Value.GetUserPaymentsAsync(user!.Id,
                (page - 1) * 5, 5);
        if (!payments.Any())
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Больше нет платежей.");
            return;
        }

        var paymentsString = string.Join("\n\n", payments.Select(payment =>
            $"Платеж <code>{payment.Id}</code>\nДата: <code>{payment.PaymentDate.ToString("g")}</code>\nСумма: <code>{payment.Cost.ToString("F1")}₽</code>"));

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId, paymentsString, ParseMode.Html,
            replyMarkup: PaymentKeyboard.ActivePayments(page));
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) => query.Data!.StartsWith("paymentsHistory");
}