using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Abstractions.Extensions;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Application.Services.Services.BotServices;
using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Jobs.Specification;
using LikeBotVK.Domain.VK.Entities;
using LikeBotVK.Domain.VK.Specification;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.CallbackQueryCommands;

public class MyJobsQueryCommand : ICallbackQueryCommand
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

        var vks = await serviceFacade.UnitOfWork.VkRepository.Value.FindAsync(new UserVksSpecification(user!.Id));

        var works = await serviceFacade.UnitOfWork.JobRepository.Value.FindAsync(
            new JobsFromVkIdsSpecification(vks.Select(vk => vk.Id).ToList()), page - 1, 1);
        if (!works.Any())
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Больше нет работ.");
            return;
        }

        var jobData = await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.GetAsync(works.First().Id);
        if (jobData?.Hashtag == null)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Не удалось получить данные о работе.");
            return;
        }

        var vk = vks.First(vk => vk.Id == works.First().VkId);

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            JobToString(works.First(), jobData, vk),
            ParseMode.Html, replyMarkup: JobsKeyboard.ActiveWorks(page, works.First()));
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) => query.Data!.StartsWith("worksHistory");


    private static string JobToString(Job job, JobData data, Vk vk)
    {
        var typeString = job.Type switch
        {
            Domain.Jobs.Enums.Type.Like => "Задача: <code>Лайк</code>",
            Domain.Jobs.Enums.Type.Subscribe => "Задача: <code>Подписка</code>",
            Domain.Jobs.Enums.Type.Repost => "Задача: <code>Репост</code>",
            _ => throw new ArgumentOutOfRangeException()
        };

        var workString =
            $"Работа №<code>{job.Id}</code>\n{typeString}\nАккаунт: <code>{vk.Username}</code>\nХештег: <code>{data.Hashtag}</code>\nИнтервал: <code>{job.LowerInterval}:{job.UpperInterval}</code>";
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
        if (data.DateTimeLimitation.HasValue)
            workString +=
                $"\nОграничение: <code>{TimeZoneInfo.ConvertTimeFromUtc(data.DateTimeLimitation.Value, tz)}</code>";
        if (job.StartTime.HasValue)
        {
            workString +=
                $"\nВремя начала: <code>{TimeZoneInfo.ConvertTimeFromUtc(job.StartTime.Value, tz).ToString("g")}</code>\nПубликаций всего: <code>{job.Publications.Count}</code>\nЗавершена: <code>{(job.IsCompleted ? "Да" : "Нет")}</code >";

            workString +=
                $"\nУспешно: <code>{job.CountSuccess}</code>";
            if (!string.IsNullOrEmpty(job.ErrorMessage))
                workString +=
                    $"\nОшибок: <code>{job.CountErrors}</code>\nПоследняя ошибка: <code>{job.ErrorMessage.ToHtmlStyle()}</code>";
        }
        else workString += "\n\n<b>Ещё не началась</b>";

        return workString;
    }
}