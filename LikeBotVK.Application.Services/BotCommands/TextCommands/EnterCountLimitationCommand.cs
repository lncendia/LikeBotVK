using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Application.Services.Services.BotServices;
using LikeBotVK.Domain.Jobs.Specification;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.TextCommands;

public class EnterCountLimitationCommand : ITextCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, Message message,
        ServiceFacade serviceFacade)
    {
        var currentJobs =
            await serviceFacade.UnitOfWork.JobRepository.Value.FindAsync(
                new JobsFromIdsSpecification(data!.CurrentJobsId));
        if (!int.TryParse(message.Text, out var count) || count < 1)
        {
            await client.SendTextMessageAsync(message.From!.Id, "Пожалуйста, введите положительное число!",
                replyMarkup: MainKeyboard.Main);
            return;
        }

        int maxCount = currentJobs.First().Type switch
        {
            Domain.Jobs.Enums.Type.Like => PublicationsCount.CountLike,
            Domain.Jobs.Enums.Type.Subscribe => PublicationsCount.CountSubscribe,
            Domain.Jobs.Enums.Type.Repost => PublicationsCount.CountRepost,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (count > maxCount)
        {
            await client.SendTextMessageAsync(message.From!.Id,
                $"Для этого типа не предусмотрено получение более {maxCount} публикаций! Попробуйте ещё раз.",
                replyMarkup: MainKeyboard.Main);
            return;
        }

        foreach (var job in data.CurrentJobsId)
        {
            var dataJob = await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.GetAsync(job);
            dataJob!.Count = count;
            await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.AddOrUpdateAsync(dataJob);
        }

        data.State = State.SelectTypeJob;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
        await client.SendTextMessageAsync(message.Chat.Id, "Введите тип работы:",
            replyMarkup: JobsKeyboard.SelectTypeWork);
    }

    public bool Compare(Message message, User? user, UserData? data) =>
        message.Type == MessageType.Text && data!.State == State.EnterCountLimitation;
}