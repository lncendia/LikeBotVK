using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Domain.VK.Entities;
using LikeBotVK.Domain.VK.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.TextCommands;

public class EnterVkDataCommand : ITextCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, Message message,
        ServiceFacade serviceFacade)
    {
        var dataVk = message.Text!.Split(':');
        if (dataVk.Length != 2)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                "Неверный формат данных! Попробуйте ещё раз.\nФормат: <code>[логин:пароль]</code>", ParseMode.Html,
                replyMarkup: MainKeyboard.Main);
            return;
        }

        Vk vk;
        try
        {
            vk = await serviceFacade.VkFactory.CreateVkAsync(user!.Id, dataVk[0], dataVk[1]);
        }
        catch (NoSubscriptionException)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "У вас недостаточно подписок.",
                replyMarkup: MainKeyboard.Main);
            return;
        }

        await serviceFacade.UnitOfWork.VkRepository.Value.AddAsync(vk);
        data!.State = State.Main;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
        await client.SendTextMessageAsync(message.Chat.Id,
            "Аккаунт успешно добавлен.", replyMarkup: VkKeyboard.Activate(vk.Id));
    }

    public bool Compare(Message message, User? user, UserData? data) =>
        message.Type == MessageType.Text && data!.State == State.EnterVkData;
}