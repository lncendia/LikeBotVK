using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Jobs.Specification;
using LikeBotVK.Domain.Jobs.Specification.Visitor;
using LikeBotVK.Domain.Specifications;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.CallbackQueryCommands;

public class ExitQueryCommand : ICallbackQueryCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, CallbackQuery query,
        ServiceFacade serviceFacade)
    {
        if (data!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        var id = int.Parse(query.Data![5..]);
        var vk = await serviceFacade.UnitOfWork.VkRepository.Value.GetAsync(id);
        if (vk == null || vk.UserId != user!.Id)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "Вы не можете выйти из этого аккаунта.");
            return;
        }

        var specification = new AndSpecification<Job, IJobSpecificationVisitor>(new JobsFromVkIdSpecification(vk.Id),
            new NotSpecification<Job, IJobSpecificationVisitor>(new FinishedJobsSpecification()));
        
        if (await serviceFacade.UnitOfWork.JobRepository.Value.CountAsync(specification) > 0)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "На этом аккаунте есть незавершенные задачи.");
            return;
        }

        await client.SendChatActionAsync(user.Id, ChatAction.Typing);
        if (!string.IsNullOrEmpty(vk.AccessToken)) await serviceFacade.VkLoginService.DeactivateAsync(vk);
        await serviceFacade.UnitOfWork.VkRepository.Value.DeleteAsync(vk);

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Аккаунт успешно удалён.");
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) => query.Data!.StartsWith("exit");
}