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

public class AdminSubscribesCommand : ITextCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, Message message,
        ServiceFacade serviceFacade)
    {
        if (data!.State is State.Main or State.MailingAdmin)
        {
            await client.SendTextMessageAsync(user!.Id,
                "Введите id человека и дату окончания подписки (111111111 11.11.2011).\nДля стандартного времени действия введите \"s\" (111111111 s).",
                replyMarkup: MainKeyboard.Main);
            data!.State = State.SubscribesAdmin;
        }
        else
        {
            await client.SendTextMessageAsync(user!.Id, "Вы вышли из панели рассылки.",
                replyMarkup: MainKeyboard.MainReplyKeyboard);
            data.State = State.Main;
        }
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
        
    }

    public bool Compare(Message message, User? user, UserData? data) =>
        message.Type == MessageType.Text && message.Text == "/subscribes" &&
        data!.State is State.Main or State.SubscribesAdmin && data.IsAdmin;
}