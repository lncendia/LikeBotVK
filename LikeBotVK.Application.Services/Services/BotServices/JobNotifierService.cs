using LikeBotVK.Application.Abstractions.Extensions;
using LikeBotVK.Application.Abstractions.Services.BotServices;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Jobs.Entities;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace LikeBotVK.Application.Services.Services.BotServices;

public class JobNotifierService : IJobNotifierService
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IUnitOfWork _unitOfWork;

    public JobNotifierService(ITelegramBotClient telegramBotClient, IUnitOfWork unitOfWork)
    {
        _telegramBotClient = telegramBotClient;
        _unitOfWork = unitOfWork;
    }

    public async Task NotifyStartAsync(Job job)
    {
        var vk = await _unitOfWork.VkRepository.Value.GetAsync(job.VkId);
        var user = await _unitOfWork.UserRepository.Value.GetAsync(vk!.UserId);
        try
        {
            await _telegramBotClient.SendTextMessageAsync(user!.Id,
                $"Запущена работа №<code>{job.Id}</code>\nАккаунт: <code>{vk.Username}</code>\n<b>Получение публикаций...</b>",
                ParseMode.Html);
        }
        catch
        {
            // ignored
        }
    }

    public async Task NotifyEndAsync(Job job)
    {
        var vk = await _unitOfWork.VkRepository.Value.GetAsync(job.VkId);
        var user = await _unitOfWork.UserRepository.Value.GetAsync(vk!.UserId);
        var message =
            $"Завершена работа №<code>{job.Id}</code>\nАккаунт: <code>{vk.Username}</code>\nУспешно: <code>{job.CountSuccess}</code>";
        if (!string.IsNullOrEmpty(job.ErrorMessage))
            message +=
                $"\nОшибок: <code>{job.CountErrors}</code>\nПоследняя ошибка: <code>{job.ErrorMessage.ToHtmlStyle()}</code>";

        try
        {
            await _telegramBotClient.SendTextMessageAsync(user!.Id, message, ParseMode.Html);
        }
        catch
        {
            // ignored
        }
    }
}