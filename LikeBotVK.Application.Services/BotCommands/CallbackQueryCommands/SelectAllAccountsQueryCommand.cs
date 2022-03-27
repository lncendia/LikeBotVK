using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.VK.Specification;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.CallbackQueryCommands;

public class SelectAllAccountsQueryCommand : ICallbackQueryCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, CallbackQuery query,
        ServiceFacade serviceFacade)
    {
        var currentWorks = await serviceFacade.UserJobService.GetUserNotStartedJobs(user!.Id);

        var vks = await serviceFacade.UnitOfWork.VkRepository.Value.FindAsync(new UserActiveVksSpecification(user.Id));
        vks.RemoveAll(vk => currentWorks.Any(job => job.VkId == vk.Id));
        vks.RemoveRange(1 + user.Subscribes.Count - currentWorks.Count, vks.Count);

        var jobs = new List<Job>();
        foreach (var vk in vks)
            jobs.Add(await serviceFacade.JobFactory.CreateJobAsync(vk.Id));

        await serviceFacade.UnitOfWork.JobRepository.Value.AddRangeAsync(jobs);

        data!.State = State.SelectActionJob;
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Выберите действие:", replyMarkup: JobsKeyboard.SelectActionType);
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) =>
        query.Data == "selectAll" && data!.State == State.SelectAccounts;
}