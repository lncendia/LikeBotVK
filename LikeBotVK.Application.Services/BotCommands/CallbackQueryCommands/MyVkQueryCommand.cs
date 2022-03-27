using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Abstractions.Extensions;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Domain.VK.Specification;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.CallbackQueryCommands;

public class MyVkQueryCommand : ICallbackQueryCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, CallbackQuery query,
        ServiceFacade serviceFacade)
    {
        if (data!.State != State.Main)
        {
            await client.AnswerCallbackQueryAsync(query.Id, "Вы должны быть в главное меню.");
            return;
        }

        var vks = await serviceFacade.UnitOfWork.VkRepository.Value.FindAsync(new UserVksSpecification(user!.Id));
        if (vks.Any())
        {
            var vk = vks.First();
            var count = vk.Password.Length / 2;
            var offsetLength = (vk.Password.Length - count) / 2;

            var password = (vk.Password[..offsetLength] + new string('*', count) +
                            vk.Password[(offsetLength + count)..]).ToHtmlStyle();
            await client.EditMessageTextAsync(query.From.Id, query.Message!.MessageId,
                $"Имя пользователя: <code>{vk.Username.ToHtmlStyle()}</code>\nПароль: <code>{password}</code>",
                ParseMode.Html, replyMarkup: VkKeyboard.VkMain(vk));
        }


        for (var i = 1; i < vks.Count; i++)
        {
            var vk = vks[i];
            var count = vk.Password.Length / 2;
            var offsetLength = (vk.Password.Length - count) / 2;

            var password = vk.Password[..offsetLength] + new string('*', count) +
                           vk.Password[(offsetLength + count)..];

            await client.SendTextMessageAsync(query.From.Id,
                $"Имя пользователя: <code>{vk.Username.ToHtmlStyle()}</code>\nПароль: <code>{password}</code>",
                ParseMode.Html, replyMarkup: VkKeyboard.VkMain(vk));
        }
    }

    public bool Compare(CallbackQuery query, User? user, UserData? data) => query.Data == "myInstagrams";
}