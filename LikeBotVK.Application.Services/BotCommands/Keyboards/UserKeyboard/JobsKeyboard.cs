using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.VK.Entities;
using Telegram.Bot.Types.ReplyMarkups;
using Type = LikeBotVK.Domain.Jobs.Enums.Type;

namespace LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;

public static class JobsKeyboard
{
    private static readonly string[] Emodji =
        {"🏞", "🏔", "🏖", "🌋", "🏜", "🏕", "🌎", "🗽", "🌃", "☘", "🐲", "🌸", "🌓", "🍃", "☀", "☁"};

    private static readonly Random Random = new();

    public static readonly InlineKeyboardMarkup Working = new(
        new List<List<InlineKeyboardButton>>
        {
            new() {InlineKeyboardButton.WithCallbackData("🏃 Начать задачу", "startWorking")},
            new() {InlineKeyboardButton.WithCallbackData("⚙ Мои задачи", "worksHistory_1")}
        });

    public static readonly InlineKeyboardMarkup SkipDataLimitation = new(
        new List<List<InlineKeyboardButton>>
        {
            new() {InlineKeyboardButton.WithCallbackData("↪ Пропустить", "skipDataLimitation")},
            new() {InlineKeyboardButton.WithCallbackData("⭐ В главное меню", "mainMenu")}
        });

    public static readonly InlineKeyboardMarkup StartWork = new(
        new List<List<InlineKeyboardButton>>
        {
            new()
            {
                InlineKeyboardButton.WithCallbackData("🏃 Начать сейчас", "startNow"),
                InlineKeyboardButton.WithCallbackData("⌛ Начать позже", "startLater")
            },
            new() {InlineKeyboardButton.WithCallbackData("🛑 Отмена", "mainMenu")}
        });

    public static InlineKeyboardMarkup Select(IEnumerable<Vk> vks)
    {
        var accounts = vks.Select(inst => new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData($"{Emodji[Random.Next(0, Emodji.Length)]} {inst.Username}",
                $"select_{inst.Id}")
        }).ToList();

        accounts.Add(new List<InlineKeyboardButton>
            {InlineKeyboardButton.WithCallbackData("🗒 Выбрать все аккаунты", "selectAll")});
        accounts.Add(new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData("👈 Подтвердить выбор", "continueSelect"),
            InlineKeyboardButton.WithCallbackData("⭐ В главное меню", "mainMenu")
        });

        return new InlineKeyboardMarkup(accounts);
    }

    public static InlineKeyboardMarkup NewSelect(List<IEnumerable<InlineKeyboardButton>?> keyboard, string query)
    {
        keyboard.Remove(keyboard.FirstOrDefault(list => list!.Any(key => key.CallbackData == query)));
        if (keyboard.Count == 2)
            keyboard.Remove(keyboard.FirstOrDefault(list => list!.Any(key => key.CallbackData == "selectAll")));

        return new InlineKeyboardMarkup(keyboard!);
    }

    public static InlineKeyboardMarkup ActiveWorks(int page, Job job)
    {
        var list = new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData("⬅", $"worksHistory_{page - 1}"),
            InlineKeyboardButton.WithCallbackData("🔄", $"workRestart_{job.Id}"),
            InlineKeyboardButton.WithCallbackData("➡", $"worksHistory_{page + 1}")
        };

        if (!job.IsCompleted)
            list.Insert(2, InlineKeyboardButton.WithCallbackData("⏹", $"stopWork_{job.Id}"));
        return new InlineKeyboardMarkup(list);
    }

    public static readonly InlineKeyboardMarkup SelectActionType = new(
        new List<List<InlineKeyboardButton>>
        {
            new()
            {
                InlineKeyboardButton.WithCallbackData("1️⃣ Лайки",
                    $"actionType_{Type.Like.ToString()}")
            },
            new()
            {
                InlineKeyboardButton.WithCallbackData("2️⃣ Репосты",
                    $"actionType_{Type.Repost.ToString()}")
            },
            new()
            {
                InlineKeyboardButton.WithCallbackData("2️⃣ Подписки",
                    $"actionType_{Type.Subscribe.ToString()}")
            },
            new() {InlineKeyboardButton.WithCallbackData("⭐ В главное меню", "mainMenu")},
        });

    public static readonly InlineKeyboardMarkup SelectTypeWork = new(
        new List<List<InlineKeyboardButton>>
        {
            new() {InlineKeyboardButton.WithCallbackData("1️⃣ Обычная", $"workType_{WorkType.Simple.ToString()}")},
            new() {InlineKeyboardButton.WithCallbackData("2️⃣ Разделить", $"workType_{WorkType.Divide.ToString()}")},
            new() {InlineKeyboardButton.WithCallbackData("⭐ В главное меню", "mainMenu")},
        });

    public static InlineKeyboardMarkup StopWork(int id) =>
        new(InlineKeyboardButton.WithCallbackData("⏹ Остановить", $"stopWork_{id}"));
}