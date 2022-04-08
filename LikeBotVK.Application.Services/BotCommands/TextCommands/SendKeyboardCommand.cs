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

public class SendKeyboardCommand : ITextCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, Message message,
        ServiceFacade serviceFacade)
    {
        if (data != null)
        {
            var currentJobs =
                await serviceFacade.UnitOfWork.JobRepository.Value.FindAsync(
                    new JobsFromIdsSpecification(data.CurrentJobsId));

            foreach (var job in currentJobs)
            {
                var jobData = await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.GetAsync(job.Id);
                if (jobData != null)
                    await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.DeleteAsync(jobData);
            }

            await serviceFacade.UnitOfWork.JobRepository.Value.DeleteRangeAsync(currentJobs);
            data.CurrentVkId = null;
            data.CurrentJobsId.Clear();
            data.State = State.Main;
        }
        else data = new UserData(user!.Id, State.Main);

        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
        await client.SendTextMessageAsync(message.From!.Id, "Вы в главном меню.",
            replyMarkup: MainKeyboard.MainReplyKeyboard);
    }

    public bool Compare(Message message, User? user, UserData? data) =>
        message.Type == MessageType.Text && message.Text!.StartsWith("/start");
}