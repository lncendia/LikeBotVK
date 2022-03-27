using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Domain.Users.ValueObjects;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = LikeBotVK.Domain.Users.Entities.User;

namespace LikeBotVK.Application.Services.BotCommands.TextCommands;

public class EnterSubscribeDataCommand : ITextCommand
{
    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, Message message,
        ServiceFacade serviceFacade)
    {
        var dataSubscribe = message.Text!.Split(' ', 2);

        if (!long.TryParse(dataSubscribe[0], out var id))
        {
            await client.SendTextMessageAsync(user!.Id, "Неверный Id. Попробуйте ещё раз.",
                replyMarkup: MainKeyboard.Main);
            return;
        }

        DateTime date;
        if (dataSubscribe[1] == "s") date = DateTime.UtcNow.AddDays(serviceFacade.Configuration.SubscribeDuration);
        else
        {
            if (!DateTime.TryParse(dataSubscribe[1], out date) || date.CompareTo(DateTime.UtcNow) <= 0)
            {
                await client.SendTextMessageAsync(user!.Id, "Неверно введена дата. Попробуйте ещё раз.",
                    replyMarkup: MainKeyboard.Main);
                return;
            }
        }

        var user2 = await serviceFacade.UnitOfWork.UserRepository.Value.GetAsync(id);

        if (user2 == null)
        {
            await client.SendTextMessageAsync(user!.Id, "Пользователь не найден. Попробуйте ещё раз.",
                replyMarkup: MainKeyboard.Main);
            return;
        }

        user2.AddSubscribe(new Subscribe(date));
        await serviceFacade.UnitOfWork.UserRepository.Value.UpdateAsync(user2);

        data!.State = State.Main;
        await serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(data);
        await client.SendTextMessageAsync(user!.Id, "Успешно. Вы в главном меню.");
        await client.SendTextMessageAsync(user2.Id, $"Администратор активировал вам подписку до {date:D}");
    }


    public bool Compare(Message message, User? user, UserData? data) => message.Type == MessageType.Text &&
                                                                        data!.State == State.SubscribesAdmin &&
                                                                        data.IsAdmin;
}