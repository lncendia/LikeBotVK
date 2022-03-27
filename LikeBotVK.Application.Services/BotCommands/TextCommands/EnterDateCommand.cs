using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Domain.Jobs.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.TextCommands;

public class EnterDateCommand : ITextCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, Message message,
        ServiceFacade serviceFacade)
    {
        if (TimeSpan.TryParse(message.Text, out var timeSpan))
        {
            var timeEnter = DateTimeOffset.Now.Add(timeSpan.Duration());
            var currentWorks = await serviceFacade.UserJobService.GetUserNotStartedJobs(user!.Id);
            if (!currentWorks.Any())
            {
                await client.SendTextMessageAsync(message.From!.Id, "Ошибка. Работы отсутсвтуют.",
                    replyMarkup: MainKeyboard.Main);
                return;
            }

            foreach (var job in currentWorks)
            {
                try
                {
                    var dataJob = await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.GetAsync(job.Id);
                    if (dataJob == null) throw new ErrorStartJobException(job, "Jobs data not found", null);
                    await serviceFacade.JobScheduler.ScheduleWorkAsync(dataJob, timeEnter);
                    await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.AddOrUpdateAsync(dataJob);
                }
                catch (ErrorStartJobException ex)
                {
                    await serviceFacade.ApplicationDataUnitOfWork.JobDataRepository.Value.DeleteAsync(job.Id);
                    await serviceFacade.UnitOfWork.JobRepository.Value.DeleteAsync(job);
                    await client.SendTextMessageAsync(message.From!.Id, $"Не удалось запустить работу: {ex.Message}.");
                }
            }

            await serviceFacade.UnitOfWork.JobRepository.Value.UpdateRangeAsync(currentWorks);

            data!.State = State.Main;
            await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
            await client.SendTextMessageAsync(message.From!.Id, "Задача успешно запущена, вы в главном меню.");
        }
    }

    public bool Compare(Message message, User? user, UserData? data) =>
        message.Type == MessageType.Text && data!.State == State.SetDate;
}