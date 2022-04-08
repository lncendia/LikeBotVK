using LikeBotVK.Application.Abstractions.DTO;
using Telegram.Bot.Types.ReplyMarkups;

namespace LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;

public static class PaymentKeyboard
{
    public static readonly InlineKeyboardMarkup Subscribes =
        new(new List<IEnumerable<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>
                {InlineKeyboardButton.WithCallbackData("➕ Оплатить подписку", "buySubscribe")},
            new List<InlineKeyboardButton>
                {InlineKeyboardButton.WithCallbackData("💵 Мои платежи", "paymentsHistory_1")},
            new List<InlineKeyboardButton> {InlineKeyboardButton.WithCallbackData("🕧 Мои подписки", "mySubscribes_1")}
        });

    public static InlineKeyboardMarkup ActivePayments(int page) =>
        new(new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData("⬅", $"paymentsHistory_{page - 1}"),
            InlineKeyboardButton.WithCallbackData("➡", $"paymentsHistory_{page + 1}")
        });

    public static readonly InlineKeyboardMarkup PaySubscribe =
        new(InlineKeyboardButton.WithCallbackData("➕ Оплатить подписку", "buySubscribe"));

    public static InlineKeyboardMarkup ActiveSubscribes(int page) =>
        new(new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData("⬅", $"mySubscribes_{page - 1}"),
            InlineKeyboardButton.WithCallbackData("➡", $"mySubscribes_{page + 1}")
        });

    public static InlineKeyboardMarkup CheckBill(PaymentData payment, int count) =>
        new(new List<List<InlineKeyboardButton>>
        {
            new() {InlineKeyboardButton.WithUrl("Оплатить", payment.PayUrl)},
            new() {InlineKeyboardButton.WithCallbackData("Проверить оплату", $"bill_{count}_{payment.BillId}")},
        });
}