using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Application.Services.Services.BotServices;
using LikeBotVK.Domain.Specifications;
using LikeBotVK.Domain.VK.Entities;
using LikeBotVK.Domain.VK.Specification;
using LikeBotVK.Domain.VK.Specification.Visitor;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.TextCommands;

public class EnterEditVkDataCommand : ITextCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, Message message,
        ServiceFacade serviceFacade)
    {
        if (!data!.CurrentVkId.HasValue)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Аккаунт не найден.", replyMarkup: MainKeyboard.Main);
            return;
        }

        var vk = await serviceFacade.UnitOfWork.VkRepository.Value.GetAsync(data.CurrentVkId.Value);
        if (vk == null)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Аккаунт не найден.", replyMarkup: MainKeyboard.Main);
            return;
        }

        var dataVk = message.Text!.Split(':');
        if (dataVk.Length != 2)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Неверный формат данных! Попробуйте ещё раз.\nФормат: <code>[логин:пароль]</code>", ParseMode.Html,
                replyMarkup: MainKeyboard.Main);
            return;
        }

        if (dataVk[0] != vk.Username)
        {
            var count = await serviceFacade.UnitOfWork.VkRepository.Value.CountAsync(
                new AndSpecification<Vk, IVkSpecificationVisitor>(new VkFromUsernameSpecification(dataVk[0]),
                    new UserVkSpecification(user!.Id)));
            if (count != 0)
            {
                await client.SendTextMessageAsync(message.Chat.Id,
                    "Аккаунт с таким логином уже есть. Введите другие данные или вернитесь в главное меню.",
                    replyMarkup: MainKeyboard.Main);
                return;
            }
        }

        vk.ChangeData(dataVk[0], dataVk[1]);
        await serviceFacade.UnitOfWork.VkRepository.Value.UpdateAsync(vk);

        data.CurrentVkId = null;
        data.State = State.Main;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);

        await client.SendTextMessageAsync(message.Chat.Id, "Аккаунт успешно изменён.");
    }

    public bool Compare(Message message, User? user, UserData? data) =>
        message.Type == MessageType.Text && data!.State == State.EnterEditVkData;
}