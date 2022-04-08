using System.Globalization;
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

public class EnterDateLimitationCommand : ITextCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, Message message,
        ServiceFacade serviceFacade)
    {
        var currentJobs =
            await serviceFacade.UnitOfWork.JobRepository.Value.FindAsync(
                new JobsFromIdsSpecification(data!.CurrentJobsId));
        if (!DateTime.TryParseExact(message.Text, "dd.MM.yy H:mm:ss", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var date))
        {
            await client.SendTextMessageAsync(message.From!.Id,
                "Неверный формат даты, попробуйте ещё раз! Формат: <code>dd.MM.yy H:mm:ss</code>",
                ParseMode.Html, replyMarkup: MainKeyboard.Main);
            return;
        }

        var tz = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
        date = TimeZoneInfo.ConvertTimeToUtc(date, tz);
        foreach (var job in currentJobs)
        {
            var dataJob = await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.GetAsync(job.Id);
            dataJob!.DateTimeLimitation = date;
            await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.AddOrUpdateAsync(dataJob);
        }

        data.State = State.SelectTypeJob;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
        await client.SendTextMessageAsync(message.Chat.Id, "Введите тип работы:",
            replyMarkup: JobsKeyboard.SelectTypeWork);
    }

    public bool Compare(Message message, User? user, UserData? data) =>
        message.Type == MessageType.Text && data!.State == State.EnterDateLimitation;
}