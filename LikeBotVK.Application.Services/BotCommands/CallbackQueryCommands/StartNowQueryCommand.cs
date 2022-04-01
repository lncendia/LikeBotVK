using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Abstractions.Exceptions;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Jobs.Specification;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.CallbackQueryCommands;

public class StartNowQueryCommand : ICallbackQueryCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, CallbackQuery query,
        ServiceFacade serviceFacade)
    {
        var currentJobs =
            await serviceFacade.UnitOfWork.JobRepository.Value.FindAsync(
                new JobsFromIdsSpecification(data!.CurrentJobsId));
        if (!currentJobs.Any())
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "Ошибка. Работы отсутсвтуют.", replyMarkup: MainKeyboard.Main);
            return;
        }

        foreach (var job in currentJobs)
        {
            try
            {
                var dataJob = await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.GetAsync(job.Id);
                if (dataJob == null) throw new ErrorStartJobException(job, "Jobs data not found", null);


                await serviceFacade.JobScheduler.StartWorkAsync(dataJob);
                await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.AddOrUpdateAsync(dataJob);
            }
            catch (ErrorStartJobException ex)
            {
                await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.DeleteAsync(job.Id);
                await serviceFacade.UnitOfWork.JobRepository.Value.DeleteAsync(job);
                await client.SendTextMessageAsync(query.Id, $"Не удалось запустить работу: {ex.Message}.");
            }
        }

        data!.State = State.Main;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Задача успешно запущена, вы в главном меню.");
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) =>
        query.Data == "startNow" && data!.State == State.SelectTimeMode;

    private async Task StartSimpleJob(Job job, ServiceFacade serviceFacade)
    {
        
    }

    private async Task StartDivideJob()
    {
    }
}