using LikeBotVK.Domain.VK.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;

public static class VkKeyboard
{
    public static InlineKeyboardMarkup Exit(long id)
    {
        var keyboard = new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData("🚪 Выйти", $"exit_{id}"),
            InlineKeyboardButton.WithCallbackData("♻ Перезайти", $"reLogIn_{id}")
        };
        return new InlineKeyboardMarkup(keyboard);
    }

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
            new() {InlineKeyboardButton.WithCallbackData("🗒 Мои аккаунты", "myInstagrams")}
        });


    public static InlineKeyboardMarkup Activate(int id)
    {
        return new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("✅ Активировать", $"active_{id}"));
    }

    public static InlineKeyboardMarkup Edit(int id)
    {
        return new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("🖊 Редактировать", $"edit_{id}"));
    }
}