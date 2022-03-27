using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Domain.Jobs.Exceptions;
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
        var jobData = await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.GetAsync(id);
        if (jobData == null)
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
        
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId, "Работа успешно остановлена.");
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) => query.Data!.StartsWith("stopWork");
}