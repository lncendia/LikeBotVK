using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.TextCommands;

public class EnterIntervalCommand : ITextCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, Message message,
        ServiceFacade serviceFacade)
    {
        var currentWorks = await serviceFacade.UserJobService.GetUserNotStartedJobs(user!.Id);
        var dataInterval = message.Text!.Split(':');
        if (dataInterval.Length != 2 || !int.TryParse(dataInterval[0], out var lower) ||
            !int.TryParse(dataInterval[1], out var upper) ||
            lower > upper || lower < 0 || upper < 0)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Неверный формат данных! Попробуйте ещё раз.\nФормат: <code>[нижний предел:верхний предел]</code>",
                ParseMode.Html, replyMarkup: MainKeyboard.Main);
            return;
        }

        if (upper > 300)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Интервал не может быть больше 5 минут!",
                replyMarkup: MainKeyboard.Main);
            return;
        }

        currentWorks.ForEach(job => job.SetInterval(lower, upper));
        await serviceFacade.UnitOfWork.JobRepository.Value.UpdateRangeAsync(currentWorks);

        data!.State = State.EnterDateLimitation;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
        await client.SendTextMessageAsync(message.From!.Id,
            "Введите дату до которой необходимо публикации (в UTC). Формат: <code>yyyy MM dd HH:mm:ss</code>",
            ParseMode.Html, replyMarkup: JobsKeyboard.SkipDataLimitation);
    }

    public bool Compare(Message message, User? user, UserData? data) =>
        message.Type == MessageType.Text && data!.State == State.EnterInterval;
}