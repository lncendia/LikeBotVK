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

public class StartWorkingQueryCommand : ICallbackQueryCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, CallbackQuery query,
        ServiceFacade serviceFacade)
    {
        if (data!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        if (data.ActiveSubscribesCount() == 0)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Для начала необходимо приобрести подписки.");
            return;
        }

        var vks = await serviceFacade.UnitOfWork.VkRepository.Value.FindAsync(
            new AndSpecification<Vk, IVkSpecificationVisitor>(new UserVksSpecification(user!.Id),
                new ActiveVksSpecification()));
        if (!vks.Any())
        {
            await client.AnswerCallbackQueryAsync(query.Id, "У вас нет активированных аккаунтов.");
            return;
        }

        var spec = new AndSpecification<Job, IJobSpecificationVisitor>(
            new JobsFromVkIdsSpecification(vks.Select(vk => vk.Id).ToList()),
            new NotSpecification<Job, IJobSpecificationVisitor>(new FinishedJobsSpecification()));
        var notFinishedJobs =
            (await serviceFacade.UnitOfWork.JobRepository.Value.FindAsync(spec)).Select(job => job.VkId).ToList();
        vks.RemoveAll(vk => notFinishedJobs.Contains(vk.Id));
        if (!vks.Any())
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Все аккаунты уже в работе.");
            return;
        }


        data.State = State.SelectAccounts;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId, "Выберите аккаунты:",
            replyMarkup: JobsKeyboard.Select(vks));
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) => query.Data == "startWorking";
}