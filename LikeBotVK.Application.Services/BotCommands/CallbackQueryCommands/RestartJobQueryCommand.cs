using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Domain.Jobs.Entities;
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

        var id = int.Parse(query.Data![14..]);
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

        var vk = await serviceFacade.UnitOfWork.VkRepository.Value.GetAsync(job.Id);

        var newJob = new Job(vk!.Id)
        {
            Type = job.Type
        };
        newJob.SetInterval(job.LowerInterval, job.UpperInterval);

        await serviceFacade.UnitOfWork.JobRepository.Value.AddAsync(newJob);

        var newData = new JobData
        {
            JobId = newJob.Id,
            Hashtag = jobData.Hashtag,
            DateTimeLimitation = jobData.DateTimeLimitation,
            WorkType = WorkType.Simple
        };
        await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.AddOrUpdateAsync(newData);

        data.State = State.SelectTimeMode;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Выберите действие:", replyMarkup: JobsKeyboard.StartWork);
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) => query.Data == "workRestart";
}