using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Application.Services.Services.BotServices;
using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Jobs.Specification;
using LikeBotVK.Domain.Jobs.Specification.Visitor;
using LikeBotVK.Domain.Specifications;
using LikeBotVK.Domain.VK.Specification;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.CallbackQueryCommands;

public class SelectAccountQueryCommand : ICallbackQueryCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, CallbackQuery query,
        ServiceFacade serviceFacade)
    {
        if (data!.CurrentJobsId.Count == data.ActiveSubscribesCount())
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы не можете выбрать больше аккаунтов.");
            return;
        }

        var id = int.Parse(query.Data![7..]);
        var vk = await serviceFacade.UnitOfWork.VkRepository.Value.GetAsync(id);
        if (vk == null || vk.UserId != user!.Id || !new ActiveVksSpecification().IsSatisfiedBy(vk))
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы не можете добавить этот аккаунт.");
            return;
        }

        var currentJobs =
            await serviceFacade.UnitOfWork.JobRepository.Value.FindAsync(
                new JobsFromIdsSpecification(data.CurrentJobsId));

        if (currentJobs.Count >= data.ActiveSubscribesCount() || currentJobs.Any(job => job.VkId == vk.Id))
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы не можете добавить этот аккаунт.");
            return;
        }

        var spec = new AndSpecification<Job, IJobSpecificationVisitor>(new JobsFromVkIdSpecification(vk.Id),
            new NotSpecification<Job, IJobSpecificationVisitor>(new FinishedJobsSpecification()));
        var countNotFinished = await serviceFacade.UnitOfWork.JobRepository.Value.CountAsync(spec);
        if (countNotFinished != 0)
        {
            await client.AnswerCallbackQueryAsync(query.Id,
                "Вы не можете выбрать аккаунт, на котором уже запущена задача.");
            return;
        }

        var job = new Job(vk.Id);
        await serviceFacade.UnitOfWork.JobRepository.Value.AddAsync(job);
        await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.AddOrUpdateAsync(new JobData(job.Id));

        data.CurrentJobsId.Add(job.Id);
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);


        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId, "Выберите аккаунты:",
            replyMarkup: JobsKeyboard.NewSelect(query.Message.ReplyMarkup!.InlineKeyboard.ToList()!, query.Data));
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) =>
        query.Data!.StartsWith("select_") && data!.State == State.SelectAccounts;
}