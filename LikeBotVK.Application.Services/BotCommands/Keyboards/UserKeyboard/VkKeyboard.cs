using LikeBotVK.Domain.VK.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;

public static class VkKeyboard
{
    private static readonly string[] Emodji =
    {
        "😌", "🚜", "👌🏾", "🎳", "🐩", "☝🏽", "🚨", "🔱", "🙊", "🐾", "🕧", "👋🏻", "🍀", "✳", "🏌‍♀", "🛬", "🎺",
        "🏄", "💏", "🏊🏻‍♀", "👏🏼", "😡", "❣"
    };

    private static readonly Random Random = new();
    // public static InlineKeyboardMarkup Exit(long id)
    // {
    //     var keyboard = new List<InlineKeyboardButton>
    //     {
    //         InlineKeyboardButton.WithCallbackData("🚪 Выйти", $"exit_{id}"),
    //         InlineKeyboardButton.WithCallbackData("♻ Перезайти", $"reLogIn_{id}")
    //     };
    //     return new InlineKeyboardMarkup(keyboard);
    // }

    public static InlineKeyboardMarkup VkMain(Vk vk)
    {
        var list = new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData("🖊", $"edit_{vk.Id}"),
        };
        List<InlineKeyboardButton> keyboard;
        if (!string.IsNullOrEmpty(vk.AccessToken))
            keyboard = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("🚪 Выйти", $"exit_{vk.Id}"),
                InlineKeyboardButton.WithCallbackData("♻ Перезайти", $"reLogIn_{vk.Id}")
            };
        else
            keyboard = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("✅ Активировать", $"active_{vk.Id}"),
                InlineKeyboardButton.WithCallbackData("🚪 Удалить", $"exit_{vk.Id}"),
            };
        return new InlineKeyboardMarkup(new List<IEnumerable<InlineKeyboardButton>> {list, keyboard});
    }

    public static readonly InlineKeyboardMarkup MyAccounts = new(
        new List<List<InlineKeyboardButton>>
        {
            new() {InlineKeyboardButton.WithCallbackData("🆕 Добавить", "enterData")},
            new() {InlineKeyboardButton.WithCallbackData("🗒 Мои аккаунты", "vkPages")}
        });

    public static readonly InlineKeyboardMarkup AddAccount =
        new(InlineKeyboardButton.WithCallbackData("🆕 Добавить", "enterData"));

    public static InlineKeyboardMarkup Activate(int id) =>
        new(InlineKeyboardButton.WithCallbackData("✅ Активировать", $"active_{id}"));

    public static InlineKeyboardMarkup Edit(int id) =>
        new(InlineKeyboardButton.WithCallbackData("🖊 Редактировать", $"edit_{id}"));

    public static InlineKeyboardMarkup VkPages(int count)
    {
        List<List<InlineKeyboardButton>> buttons = new();
        count /= 10;
        count++;
        for (int i = 0; i < count; i++)
        {
            buttons.Add(new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData($"{Emodji[Random.Next(0, Emodji.Length)]} Страница {i + 1}",
                    $"myVks_{i+1}")
            });
        }

        return new InlineKeyboardMarkup(buttons);
    }
}