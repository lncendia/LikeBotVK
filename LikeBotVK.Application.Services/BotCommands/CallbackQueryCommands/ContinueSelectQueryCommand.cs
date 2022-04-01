using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.CallbackQueryCommands;

public class ContinueSelectQueryCommand : ICallbackQueryCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, CallbackQuery query,
        ServiceFacade serviceFacade)
    {
        if (!data!.CurrentJobsId.Any())
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы не выбрали ни одного аккаунта.");
            return;
        }

        data.State = State.SelectActionJob;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Выберите действие:", replyMarkup: JobsKeyboard.SelectActionType);
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) =>
        query.Data == "continueSelect" && data!.State == State.SelectAccounts;
}