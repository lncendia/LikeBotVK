using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Domain.Jobs.Enums;
using LikeBotVK.Domain.Jobs.Specification;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Type = LikeBotVK.Domain.Jobs.Enums.Type;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.CallbackQueryCommands;

public class SelectActionTypeQueryCommand : ICallbackQueryCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, CallbackQuery query,
        ServiceFacade serviceFacade)
    {
        var type = (Type) Enum.Parse(typeof(Type), query.Data![11..]);
        var currentJobs =
            await serviceFacade.UnitOfWork.JobRepository.Value.FindAsync(
                new JobsFromIdsSpecification(data!.CurrentJobsId));
        currentJobs.ForEach(job => job.Type = type);
        await serviceFacade.UnitOfWork.JobRepository.Value.UpdateRangeAsync(currentJobs);
        data!.State = State.EnterHashtag;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Введите хештег по которому необходимо получить публикации.", replyMarkup: MainKeyboard.Main);
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) =>
        query.Data!.StartsWith("actionType") && data!.State == State.SelectActionJob;
}