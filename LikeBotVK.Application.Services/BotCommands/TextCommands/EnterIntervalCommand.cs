using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Application.Services.Services.BotServices;
using LikeBotVK.Domain.Jobs.Specification;
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
        var currentJobs =
            await serviceFacade.UnitOfWork.JobRepository.Value.FindAsync(
                new JobsFromIdsSpecification(data!.CurrentJobsId));
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

        currentJobs.ForEach(job => job.SetInterval(lower, upper));
        await serviceFacade.UnitOfWork.JobRepository.Value.UpdateRangeAsync(currentJobs);

        data.State = State.EnterCountLimitation;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
        await client.SendTextMessageAsync(message.From!.Id,
            "Введите количество публикаций, которое необходимо получить.",
            replyMarkup: JobsKeyboard.SkipDataLimitation);
    }

    public bool Compare(Message message, User? user, UserData? data) =>
        message.Type == MessageType.Text && data!.State == State.EnterInterval;
}