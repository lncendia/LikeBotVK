using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
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
        var jobs = await serviceFacade.UserJobService.GetUserNotStartedJobs(user!.Id);
        try
        {
            if (jobs.Any())
            {
                foreach (var job in jobs)
                    await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.DeleteAsync(job.Id);

                await serviceFacade.UnitOfWork.JobRepository.Value.DeleteRangeAsync(jobs);
            }
        }
        catch (Exception ex)
        {
            await client.SendTextMessageAsync(message.From!.Id, $"Ошибка: {ex.Message}",
                replyMarkup: MainKeyboard.Main);
            return;
        }

        data ??= new UserData {UserId = user.Id, State = State.Main};

        data.CurrentVkId = null;
        data.State = State.Main;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
        await client.SendTextMessageAsync(message.From!.Id, "Вы в главном меню.",
            replyMarkup: MainKeyboard.MainReplyKeyboard);
    }

    public bool Compare(Message message, User? user, UserData? data) =>
        message.Type == MessageType.Text && message.Text!.StartsWith("/start");
}