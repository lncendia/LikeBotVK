using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Abstractions.Exceptions;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Application.Services.Services.BotServices;
using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Jobs.Specification;
using LikeBotVK.Domain.Jobs.Specification.Visitor;
using LikeBotVK.Domain.Specifications;
using LikeBotVK.Domain.VK.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.CallbackQueryCommands;

public class ReLogInQueryCommand : ICallbackQueryCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, CallbackQuery query,
        ServiceFacade serviceFacade)
    {
        if (data!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        var id = int.Parse(query.Data![8..]);
        var vk = await serviceFacade.UnitOfWork.VkRepository.Value.GetAsync(id);
        if (vk == null || vk.UserId != user!.Id)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "Вы не можете выйти из этого аккаунта.");
            return;
        }

        var specification = new AndSpecification<Job, IJobSpecificationVisitor>(new JobsFromVkIdSpecification(vk.Id),
            new NotSpecification<Job, IJobSpecificationVisitor>(new StartedJobsSpecification()));
        var notStartedJobs = await serviceFacade.UnitOfWork.JobRepository.Value.FindAsync(specification);
        foreach (var job in notStartedJobs)
        {
            var jobData = await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.GetAsync(job.Id);
            if (jobData != null) await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.DeleteAsync(jobData);
        }
        await serviceFacade.UnitOfWork.JobRepository.Value.DeleteRangeAsync(notStartedJobs);
        
        specification = new AndSpecification<Job, IJobSpecificationVisitor>(new JobsFromVkIdSpecification(vk.Id),
            new NotSpecification<Job, IJobSpecificationVisitor>(new FinishedJobsSpecification()));
        if (await serviceFacade.UnitOfWork.JobRepository.Value.CountAsync(specification) > 0)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "На этом аккаунте есть незавершенные задачи.");
            return;
        }
        
        


        await serviceFacade.VkLoginService.DeactivateAsync(vk);
        vk.AccessToken = null;
        await serviceFacade.UnitOfWork.VkRepository.Value.UpdateAsync(vk);


        await client.SendChatActionAsync(user.Id, ChatAction.Typing);
        LoginResult result;
        try
        {
            await CheckProxyAsync(vk, serviceFacade);
            result = await serviceFacade.VkLoginService.ActivateAsync(vk);
        }
        catch (ErrorActiveVkException ex)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                $"Не удалось активировать аккаунт: {ex.Message}.");
            return;
        }

        switch (result)
        {
            case LoginResult.Succeeded:
                await serviceFacade.UnitOfWork.VkRepository.Value.UpdateAsync(vk);
                await client.SendTextMessageAsync(query.From.Id, "Аккаунт успешно активирован.");
                data.State = State.Main;
                break;
            case LoginResult.BadData:
                await client.SendTextMessageAsync(query.From.Id, "Введены неверные данные.",
                    replyMarkup: VkKeyboard.Edit(vk.Id));
                data.State = State.Main;
                break;
            case LoginResult.TwoFactorRequired:
                data.CurrentVkId = vk.Id;
                data.State = State.EnterTwoFactorCode;
                await client.SendTextMessageAsync(query.From.Id, "Введите код двухфакторной авторизации.",
                    replyMarkup: MainKeyboard.Main);
                break;
            default:
                await client.SendTextMessageAsync(query.From.Id,
                    "Ошибка при отправке запроса. Попробуйте войти ещё раз.");
                data.State = State.Main;
                break;
        }

        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) => query.Data!.StartsWith("reLogIn");
    
    private static async Task CheckProxyAsync(Vk vk, ServiceFacade facade)
    {
        if (vk.ProxyId == null)
            if (await facade.ProxySetter.SetProxyAsync(vk))
                await facade.UnitOfWork.VkRepository.Value.UpdateAsync(vk);
    }
}