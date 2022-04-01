using System.Collections;
using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.BotServices;
using LikeBotVK.Application.Abstractions.Repositories;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using Telegram.Bot;

namespace LikeBotVK.Application.Services.Services;

public class SubscribeDeleter : ISubscribeDeleter
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IApplicationDataUnitOfWork _applicationDataUnitOfWork;

    public SubscribeDeleter(ITelegramBotClient telegramBotClient, IApplicationDataUnitOfWork applicationDataUnitOfWork)
    {
        _telegramBotClient = telegramBotClient;
        _applicationDataUnitOfWork = applicationDataUnitOfWork;
    }

    public async Task DeleteAsync()
    {
        var users = await _applicationDataUnitOfWork.UserDataRepository.Value.GetUsersWithExpiredSubscribesAsync();
        foreach (var user in users)
        {
            //TODO:Subscribes
            var expiredSubscribes = user.Subscribes.Where(s => s.IsExpired()).ToList();
            await NotifyAsync(expiredSubscribes, user);
            expiredSubscribes.ForEach(u => user.Subscribes.Remove(u));
            await _applicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(user);
        }
    }

    private async Task NotifyAsync(ICollection subscribes, UserData user)
    {
        var workString = $"Завершилась действие {subscribes.Count} подписки(ок).\nПродлите, нажав на кнопку.";

        try
        {
            await _telegramBotClient.SendTextMessageAsync(user.UserId, workString,
                replyMarkup: PaymentKeyboard.PaySubscribe);
        }
        catch
        {
            //ignored
        }
    }
}