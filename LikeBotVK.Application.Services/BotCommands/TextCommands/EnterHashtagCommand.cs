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

public class EnterHashtagCommand : ITextCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, Message message,
        ServiceFacade serviceFacade)
    {
        var currentJobs =
            await serviceFacade.UnitOfWork.JobRepository.Value.FindAsync(
                new JobsFromIdsSpecification(data!.CurrentJobsId));
        var hashtag = message.Text!.Trim(' ');
        if (hashtag[0] != '#') hashtag = '#' + hashtag;

        foreach (var job in currentJobs)
        {
            var dataJob = await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.GetAsync(job.Id);
            dataJob!.Hashtag = hashtag;
            await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.AddOrUpdateAsync(dataJob);
        }

        data.State = State.EnterInterval;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
        await client.SendTextMessageAsync(message.Chat.Id,
            "Введите интервал действий в формате <code>[нижний предел:верхний предел]</code>", ParseMode.Html,
            replyMarkup: MainKeyboard.Main);
    }

    public bool Compare(Message message, User? user, UserData? data) =>
        message.Type == MessageType.Text && data!.State == State.EnterHashtag;
}