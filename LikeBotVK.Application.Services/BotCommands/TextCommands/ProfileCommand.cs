using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.TextCommands
{
    public class ProfileCommand : ITextCommand
    {
        public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, Message message,
            ServiceFacade serviceFacade)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                $"<b>Ваш Id:</b> {user!.Id}\n<b>Бонусный счет:</b> {data!.BonusAccount}₽\n<b>Реферальная ссылка:</b> https://telegram.me/{(await client.GetMeAsync()).Username}?start={user.Id}",
                ParseMode.Html, disableWebPagePreview: true);
        }

        public bool Compare(Message message, User? user, UserData? data) => message.Type == MessageType.Text &&
                                                                            message.Text == "🗒 Мой профиль" &&
                                                                            data!.State == State.Main;
    }
}