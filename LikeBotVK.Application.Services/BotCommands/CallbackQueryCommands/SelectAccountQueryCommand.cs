using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Domain.Jobs.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.CallbackQueryCommands;

public class SelectAccountQueryCommand : ICallbackQueryCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, CallbackQuery query,
        ServiceFacade serviceFacade)
    {
        var id = int.Parse(query.Data![7..]);
        var vk = await serviceFacade.UnitOfWork.VkRepository.Value.GetAsync(id);
        if (vk == null || vk.UserId != user!.Id || string.IsNullOrEmpty(vk.AccessToken))
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы не можете добавить этот аккаунт.");
            return;
        }


        var currentWorks = await serviceFacade.UserJobService.GetUserNotStartedJobs(user.Id);

        if (currentWorks.Count >= user.Subscribes.Count || currentWorks.Any(job => job.VkId == vk.Id))
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы не можете добавить этот аккаунт.");
            return;
        }

        var job = await serviceFacade.JobFactory.CreateJobAsync(vk.Id);
        await serviceFacade.UnitOfWork.JobRepository.Value.AddAsync(job);

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId, "Выберите аккаунты:",
            replyMarkup: JobsKeyboard.NewSelect(query.Message.ReplyMarkup!.InlineKeyboard.ToList()!, query.Data));
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) =>
        query.Data!.StartsWith("select") && data!.State == State.SelectAccounts;
}