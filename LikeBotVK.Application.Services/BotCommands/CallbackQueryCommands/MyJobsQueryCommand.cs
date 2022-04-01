using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Abstractions.Extensions;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
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

        var vk = vks.First(vk => vk.Id == works.First().VkId);

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId, JobToString(works.First(), vk),
            ParseMode.Html, replyMarkup: JobsKeyboard.ActiveWorks(page, works.First()));
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) => query.Data!.StartsWith("worksHistory");


    private static string JobToString(Job job, Vk vk)
    {
        var typeString = job.Type switch
        {
            Domain.Jobs.Enums.Type.Like => "Задача: <code>Лайк</code>",
            Domain.Jobs.Enums.Type.Subscribe => "Задача: <code>Подписка</code>",
            Domain.Jobs.Enums.Type.Repost => "Задача: <code>Репост</code>",
            _ => throw new ArgumentOutOfRangeException()
        };

        var workString =
            $"Работа №<code>{job.Id}</code>\n{typeString}\nАккаунт: <code>{vk.Username}</code>\nИнтервал: <code>{job.LowerInterval}:{job.UpperInterval}</code>";

        if (job.StartTime.HasValue)
            workString +=
                $"\nВремя начала: <code>{job.StartTime.Value.ToString("g")}</code>\nПользователей всего: <code>{job.Publications.Count}</code>\nЗавершена: <code>{(job.IsCompleted ? "Да" : "Нет")}</code >\n";

        if (!string.IsNullOrEmpty(job.ErrorMessage))
            workString += $"\nПоследняя ошибка: <code>{job.ErrorMessage.ToHtmlStyle()}</code>";
        if (job.IsCompleted)
            workString += $"\nУспешно: <code>{(job.CountErrors < job.CountSuccess ? "Да" : "Нет")}</code>";

        return workString;
    }
}