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

public class AccountsCommand : ITextCommand
{
    private readonly Random _rnd = new();

    public async Task ExecuteAsync(ITelegramBotClient client, User? user, UserData? data, Message message,
        ServiceFacade serviceFacade)
    {
        await client.SendTextMessageAsync(message.Chat.Id, "Выберите, что вы хотите сделать.",
            replyMarkup: VkKeyboard.MyAccounts);
    }

    public bool Compare(Message message, User? user, UserData? data) => message.Type == MessageType.Text &&
                                                                        message.Text == "🌇 Мои аккаунты" &&
                                                                        data!.State == State.Main;
}