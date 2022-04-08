using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Abstractions.Exceptions;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.Services.BotServices;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.CallbackQueryCommands;

public class StopWorkQueryCommand : ICallbackQueryCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, CallbackQuery query,
        ServiceFacade serviceFacade)
    {
        if (data!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        var id = int.Parse(query.Data![9..]);
        var jobTask = serviceFacade.UnitOfWork.JobRepository.Value.GetAsync(id);
        var jobDataTask = serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.GetAsync(id);
        await Task.WhenAll(jobTask, jobDataTask);
        var job = jobTask.Result;
        var jobData = jobDataTask.Result;
        if (job == null || jobData == null)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "Вы не можете остановить эту работу.");
            return;
        }

        try
        {
            await serviceFacade.JobScheduler.CancelWorkAsync(jobData);
        }
        catch (ErrorStopJobException ex)
        {
            await client.AnswerCallbackQueryAsync(query.Id, $"Не удалось отсановить работу: {ex.Message}.", true);
            return;
        }

        job.MarkAsCompleted();
        await serviceFacade.UnitOfWork.JobRepository.Value.UpdateAsync(job);

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId, "Работа успешно остановлена.");
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) => query.Data!.StartsWith("stopWork");
}