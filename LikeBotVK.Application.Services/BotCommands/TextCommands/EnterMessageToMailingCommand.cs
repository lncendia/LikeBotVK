using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.TextCommands;

public class EnterMessageToMailingCommand : ITextCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, Message message,
        ServiceFacade serviceFacade)
    {
        var users = await serviceFacade.UnitOfWork.UserRepository.Value.FindAsync(null);
        IEnumerable<Task<Message>> tasks;
        switch (message.Type)
        {
            case MessageType.Text:
                tasks = users.Select(user1 =>
                    client.SendTextMessageAsync(user1.Id, message.Text!, ParseMode.Html));
                break;
            case MessageType.Photo:
                tasks = users.Select(user1 => client.SendPhotoAsync(user1.Id,
                    new InputMedia(message.Photo!.Last().FileId), message.Caption, ParseMode.Html));
                break;
            case MessageType.Audio:
                tasks = users.Select(user1 =>
                    client.SendAudioAsync(user1.Id, new InputMedia(message.Audio!.FileId), parseMode: ParseMode.Html));
                break;
            case MessageType.Video:
                tasks = users.Select(user1 => client.SendVideoAsync(user1.Id, new InputMedia(message.Video!.FileId),
                    caption: message.Caption, parseMode: ParseMode.Html));
                break;
            case MessageType.Voice:
                tasks = users.Select(user1 =>
                    client.SendVoiceAsync(user1.Id, new InputMedia(message.Voice!.FileId), parseMode: ParseMode.Html));
                break;
            case MessageType.Document:
                tasks = users.Select(user1 => client.SendDocumentAsync(user1.Id,
                    new InputMedia(message.Document!.FileId), caption: message.Caption, parseMode: ParseMode.Html));
                break;
            case MessageType.Sticker:
                tasks = users.Select(user1 =>
                    client.SendStickerAsync(user1.Id, new InputMedia(message.Sticker!.FileId)));
                break;
            default:
                await client.SendTextMessageAsync(user!.Id, "Формат сообщения не поддерживается. Попробуйте ещё раз.",
                    replyMarkup: MainKeyboard.Main);
                return;
        }

        var task = Task.WhenAll(tasks);
        try
        {
            await task;
            await client.SendTextMessageAsync(user!.Id,
                $"Сообщение было успешно отправлено {users.Count} пользователю(ям). Вы в главном меню.");
        }
        catch (Exception)
        {
            var exceptionsCount = task.Exception?.InnerExceptions.Count ?? 0;
            await client.SendTextMessageAsync(user!.Id,
                $"Сообщение было отправлено {users.Count - exceptionsCount} пользователю(ям). У {exceptionsCount} пользователя(ей) возникла ошибка. Вы в главном меню.");
        }

        GC.Collect();
        data!.State = State.Main;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
    }

    public bool Compare(Message message, User? user, UserData? data) => data!.State == State.MailingAdmin;
}