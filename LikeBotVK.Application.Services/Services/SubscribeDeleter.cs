using System.Collections;
using LikeBotVK.Application.Abstractions.BotServices;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Users.Entities;
using LikeBotVK.Domain.Users.Specification;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace LikeBotVK.Application.Services.Services;

public class SubscribeDeleter : ISubscribeDeleter
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IUnitOfWork _unitOfWork;

    public SubscribeDeleter(ITelegramBotClient telegramBotClient, IUnitOfWork unitOfWork)
    {
        _telegramBotClient = telegramBotClient;
        _unitOfWork = unitOfWork;
    }

    public async Task DeleteAsync()
    {
        var users = await _unitOfWork.UserRepository.Value.FindAsync(new UsersWithExpiredSubscribesSpecification());
        foreach (var user in users)
        {
            //TODO:Subscribes
            var expiredSubscribes = user.GetExpiredSubscribes();
            await NotifyAsync(expiredSubscribes, user);
            expiredSubscribes.ForEach(u => user.RemoveSubscribe(u));
            await _unitOfWork.UserRepository.Value.UpdateAsync(user);
        }
    }

    private async Task NotifyAsync(ICollection subscribes, User user)
    {
        var workString = $"Завершилась действие {subscribes.Count} подписки(ок).\nПродлите, нажав на кнопку.";

        try
        {
            await _telegramBotClient.SendTextMessageAsync(user.Id, workString,
                replyMarkup: PaymentKeyboard.PaySubscribe);
        }
        catch
        {
            //ignored
        }
    }
}