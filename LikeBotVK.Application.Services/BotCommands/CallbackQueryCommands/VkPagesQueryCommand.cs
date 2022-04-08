using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Application.Services.Services.BotServices;
using LikeBotVK.Domain.VK.Specification;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.CallbackQueryCommands;

public class VkPagesQueryCommand : ICallbackQueryCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, CallbackQuery query,
        ServiceFacade serviceFacade)
    {
        if (data!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        var vks = await serviceFacade.UnitOfWork.VkRepository.Value.CountAsync(new UserVksSpecification(user!.Id));
        if (vks == 0)
        {
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                "У вас нет аккаунтов.", replyMarkup: VkKeyboard.AddAccount);
            return;
        }

        await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
            "Выберите страницу:", replyMarkup: VkKeyboard.VkPages(vks));
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) => query.Data == "vkPages";
}