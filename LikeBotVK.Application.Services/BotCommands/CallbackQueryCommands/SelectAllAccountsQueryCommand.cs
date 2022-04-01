using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Jobs.Specification;
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
        var currentJobs =
            await serviceFacade.UnitOfWork.JobRepository.Value.FindAsync(
                new JobsFromIdsSpecification(data!.CurrentJobsId));

        var vks = await serviceFacade.UnitOfWork.VkRepository.Value.FindAsync(
            new AndSpecification<Vk, IVkSpecificationVisitor>(new UserVksSpecification(user!.Id),
                new ActiveVksSpecification()));
        vks.RemoveAll(vk => currentJobs.Any(job => job.VkId == vk.Id));

        var jobs = vks.Take(data.ActiveSubscribesCount()).Select(vk => new Job(vk.Id)).ToList();

        await serviceFacade.UnitOfWork.JobRepository.Value.AddRangeAsync(jobs);

        data.State = State.SelectActionJob;
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Выберите действие:", replyMarkup: JobsKeyboard.SelectActionType);
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) =>
        query.Data == "selectAll" && data!.State == State.SelectAccounts;
}