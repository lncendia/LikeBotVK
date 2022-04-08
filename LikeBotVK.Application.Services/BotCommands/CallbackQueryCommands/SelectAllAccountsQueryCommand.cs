using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Application.Services.Services.BotServices;
using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Jobs.Specification;
using LikeBotVK.Domain.Jobs.Specification.Visitor;
using LikeBotVK.Domain.Specifications;
using LikeBotVK.Domain.VK.Entities;
using LikeBotVK.Domain.VK.Specification;
using LikeBotVK.Domain.VK.Specification.Visitor;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.CallbackQueryCommands;

public class SelectAllAccountsQueryCommand : ICallbackQueryCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, CallbackQuery query,
        ServiceFacade serviceFacade)
    {
        if (data!.CurrentJobsId.Count == data.ActiveSubscribesCount())
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы не можете выбрать больше аккаунтов.");
            return;
        }

        var currentJobs =
            await serviceFacade.UnitOfWork.JobRepository.Value.FindAsync(
                new JobsFromIdsSpecification(data.CurrentJobsId));

        var vks = await serviceFacade.UnitOfWork.VkRepository.Value.FindAsync(
            new AndSpecification<Vk, IVkSpecificationVisitor>(new UserVksSpecification(user!.Id),
                new ActiveVksSpecification()));
        vks.RemoveAll(vk => currentJobs.Any(job => job.VkId == vk.Id));


        var spec = new AndSpecification<Job, IJobSpecificationVisitor>(
            new JobsFromVkIdsSpecification(vks.Select(vk => vk.Id).ToList()),
            new NotSpecification<Job, IJobSpecificationVisitor>(new FinishedJobsSpecification()));
        var notFinishedJobs =
            (await serviceFacade.UnitOfWork.JobRepository.Value.FindAsync(spec)).Select(job => job.VkId).ToList();
        vks.RemoveAll(vk => notFinishedJobs.Contains(vk.Id));
        var jobs = vks.Take(data.ActiveSubscribesCount()).Select(vk => new Job(vk.Id)).ToList();

        if (!jobs.Any())
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Нет доступных аккаунтов.");
            return;
        }

        await serviceFacade.UnitOfWork.JobRepository.Value.AddRangeAsync(jobs);
        foreach (var job in jobs)
            await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.AddOrUpdateAsync(new JobData(job.Id));

        data.CurrentJobsId = jobs.Select(x => x.Id).ToList();
        data.State = State.SelectActionJob;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Выберите действие:", replyMarkup: JobsKeyboard.SelectActionType);
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) =>
        query.Data == "selectAll" && data!.State == State.SelectAccounts;
}