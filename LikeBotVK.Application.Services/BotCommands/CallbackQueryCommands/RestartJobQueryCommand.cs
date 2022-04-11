using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Application.Services.Services.BotServices;
using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Jobs.Specification;
using LikeBotVK.Domain.Jobs.Specification.Visitor;
using LikeBotVK.Domain.Specifications;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.CallbackQueryCommands;

public class RestartJobQueryCommand : ICallbackQueryCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, CallbackQuery query,
        ServiceFacade serviceFacade)
    {
        if (data!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        var id = int.Parse(query.Data![12..]);
        var t1 = serviceFacade.UnitOfWork.JobRepository.Value.GetAsync(id);
        var t2 = serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.GetAsync(id);
        await Task.WhenAll(t1, t2);
        var job = t1.Result;
        var jobData = t2.Result;
        if (job == null || jobData == null)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "Вы не можете перезапустить эту работу.");
            return;
        }


        var spec = new AndSpecification<Job, IJobSpecificationVisitor>(new JobsFromVkIdSpecification(job.VkId),
            new NotSpecification<Job, IJobSpecificationVisitor>(new FinishedJobsSpecification()));
        var countNotFinished = await serviceFacade.UnitOfWork.JobRepository.Value.CountAsync(spec);
        if (countNotFinished != 0)
        {
            await client.AnswerCallbackQueryAsync(query.Id,
                "Вы не можете выбрать аккаунт, на котором уже запущена задача.");
            return;
        }

        var vk = await serviceFacade.UnitOfWork.VkRepository.Value.GetAsync(job.VkId);

        var newJob = new Job(vk!.Id)
        {
            Type = job.Type
        };
        newJob.SetInterval(job.LowerInterval, job.UpperInterval);

        await serviceFacade.UnitOfWork.JobRepository.Value.AddAsync(newJob);
        var newData = new JobData(newJob.Id) {Hashtag = jobData.Hashtag};
        await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.AddOrUpdateAsync(newData);

        data.CurrentJobsId.Add(newJob.Id);
        data.State = State.EnterCountLimitation;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Введите количество публикаций, которое необходимо получить.",
            replyMarkup: JobsKeyboard.SkipDataLimitation);
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) => query.Data!.StartsWith("workRestart");
}