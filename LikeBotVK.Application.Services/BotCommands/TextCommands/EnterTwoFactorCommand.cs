using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Application.Services.Services.BotServices;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.TextCommands;

public class EnterTwoFactorCommand : ITextCommand
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

        await client.SendChatActionAsync(user!.Id, ChatAction.Typing);
        LoginResult result;
        try
        {
            result = await serviceFacade.VkLoginService.EnterTwoFactorAsync(vk, message.Text!);
        }
        catch (Exception ex)
        {
            await client.SendTextMessageAsync(message.From!.Id,
                $"Ошибка при отправке запроса ({ex.Message}). Попробуйте войти ещё раз.");
            data.State = State.Main;
            await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
            return;
        }

        switch (result)
        {
            case LoginResult.Succeeded:
                await serviceFacade.UnitOfWork.VkRepository.Value.UpdateAsync(vk);
                await client.SendTextMessageAsync(message.From!.Id, "Аккаунт успешно активирован.");
                data!.State = State.Main;
                break;
            case LoginResult.BadData:
                await client.SendTextMessageAsync(message.From!.Id, "Введены неверные данные.",
                    replyMarkup: VkKeyboard.Edit(vk.Id));
                data!.State = State.Main;
                break;
            case LoginResult.TwoFactorRequired:
                await client.SendTextMessageAsync(message.From!.Id, "Введён неверный код. Попробуйте ещё раз.",
                    replyMarkup: MainKeyboard.Main);
                return;
            default:
                await client.SendTextMessageAsync(message.From!.Id,
                    "Ошибка при отправке запроса. Попробуйте войти ещё раз.");
                data!.State = State.Main;
                break;
        }

        data.CurrentVkId = null;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
    }

    public bool Compare(Message message, User? user, UserData? data) =>
        message.Type == MessageType.Text && data!.State == State.EnterTwoFactorCode;
}