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

public class MySubscribesQueryCommand : ICallbackQueryCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, CallbackQuery query,
        ServiceFacade serviceFacade)
    {
        if (data!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        var page = int.Parse(query.Data![13..]);
        if (page < 1)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы в конце списка.");
            return;
        }

        var payments = data.Subscribes.Skip((page - 1) * 10).Take(10).ToList();
        if (!payments.Any())
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Больше нет подписок.");
            return;
        }

        var tz = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");

        var paymentsString = string.Join("\n\n", payments.Select(payment =>
            $"Дата окончания: <code>{TimeZoneInfo.ConvertTimeFromUtc(payment.EndSubscribe, tz):g}</code>"));

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId, paymentsString, ParseMode.Html,
            replyMarkup: PaymentKeyboard.ActiveSubscribes(page));
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) => query.Data!.StartsWith("mySubscribes");
}