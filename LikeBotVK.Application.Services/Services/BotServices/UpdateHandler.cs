using LikeBotVK.Application.Abstractions.Configuration;
using LikeBotVK.Application.Abstractions.Repositories;
using LikeBotVK.Application.Abstractions.Services.BotServices;
using LikeBotVK.Application.Services.BotCommands.CallbackQueryCommands;
using LikeBotVK.Application.Services.BotCommands.Interfaces;
using LikeBotVK.Application.Services.BotCommands.Keyboards.UserKeyboard;
using LikeBotVK.Application.Services.BotCommands.TextCommands;
using LikeBotVK.Domain.Abstractions.Repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LikeBotVK.Application.Services.Services.BotServices;

public class UpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<UpdateHandler> _logger;
    private readonly ServiceFacade _serviceFacade;

    public UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger, IVkLoginService vkLoginService,
        Configuration configuration, IPaymentCreatorService paymentService,
        IJobScheduler jobScheduler, IUnitOfWork unitOfWork, IApplicationDataUnitOfWork applicationDataUnitOfWork,
        IProxySetter proxySetter)

    {
        _botClient = botClient;
        _logger = logger;
        _serviceFacade = new ServiceFacade(vkLoginService, configuration, paymentService, jobScheduler,
            unitOfWork, applicationDataUnitOfWork, proxySetter);
    }

    private static readonly List<ITextCommand> TextCommands = new()
    {
        new StartCommand(),
        new AccountsCommand(),
        new AdminMailingCommand(),
        new AdminSubscribesCommand(),
        new BanCommand(),
        new EnterCountSubscribesCommand(),
        new EnterDateCommand(),
        new EnterCountLimitationCommand(),
        new EnterEditVkDataCommand(),
        new EnterHashtagCommand(),
        new EnterIntervalCommand(),
        new EnterMessageToMailingCommand(),
        new EnterSubscribeDataCommand(),
        new EnterTwoFactorCommand(),
        new EnterVkDataCommand(),
        new HelpCommand(),
        new InstructionCommand(),
        new OurProjectsCommand(),
        new PaymentCommand(),
        new SendKeyboardCommand(),
        new WorkCommand(),
        new ProfileCommand()
    };

    private static readonly List<ICallbackQueryCommand> CallbackQueryCommands = new()
    {
        new ActiveVkQueryCommand(),
        new BuySubscribeQueryCommand(),
        new BillQueryCommand(),
        new VkPagesQueryCommand(),
        new ContinueSelectQueryCommand(),
        new EditVkQueryCommand(),
        new ExitQueryCommand(),
        new MainMenuQueryCommand(),
        new MyJobsQueryCommand(),
        new MyPaymentsQueryCommand(),
        new MySubscribesQueryCommand(),
        new MyVkQueryCommand(),
        new ReLogInQueryCommand(),
        new RestartJobQueryCommand(),
        new SelectAccountQueryCommand(),
        new SelectActionTypeQueryCommand(),
        new SelectAllAccountsQueryCommand(),
        new SelectWorkTypeQueryCommand(),
        new SkipCountLimitationQueryCommand(),
        new StartEnterAccountDataQueryCommand(),
        new StartLaterQueryCommand(),
        new StartNowQueryCommand(),
        new StartWorkingQueryCommand(),
        new StopWorkQueryCommand()
    };

    public async Task HandleAsync(Update update)
    {
        var handler = update.Type switch
        {
            // UpdateType.Unknown:
            // UpdateType.ChannelPost:
            // UpdateType.EditedChannelPost:
            // UpdateType.ShippingQuery:
            // UpdateType.PreCheckoutQuery:
            // UpdateType.Poll:
            UpdateType.Message => BotOnMessageReceived(update.Message!),
            UpdateType.CallbackQuery => BotOnCallbackQueryReceived(update.CallbackQuery!),
            _ => UnknownUpdateHandlerAsync()
        };

        try
        {
            await handler;
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(update, exception);
        }
    }

    private async Task HandleErrorAsync(Update update, Exception ex)
    {
        var id = update.Type switch
        {
            UpdateType.Message => update.Message!.From!.Id,
            UpdateType.CallbackQuery => update.CallbackQuery!.From.Id,
            _ => 0
        };
        if (id != 0)
        {
            try
            {
                await _botClient.SendTextMessageAsync(id,
                    $"Возникла ошибка при обработке команды: ({ex.GetType()}) {ex.Message} За дополнительной информацией обратитесь в поддержку.",
                    replyMarkup: MainKeyboard.Main);
            }
            catch
            {
                // ignored
            }
        }

        _logger.LogError(ex, "Update id: {Id}", update.Id);
    }

    private Task UnknownUpdateHandlerAsync()
    {
        return Task.CompletedTask;
    }

    private async Task BotOnCallbackQueryReceived(CallbackQuery updateCallbackQuery)
    {
        var user = await _serviceFacade.UnitOfWork.UserRepository.Value.GetAsync(updateCallbackQuery.From.Id);
        var data =
            await _serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.GetAsync(
                updateCallbackQuery.From.Id);

        var command = CallbackQueryCommands.FirstOrDefault(command => command.Compare(updateCallbackQuery, user, data));
        if (command != null)
            await command.ExecuteAsync(_botClient, user, data, updateCallbackQuery, _serviceFacade);
    }

    private async Task BotOnMessageReceived(Message updateMessage)
    {
        var user = await _serviceFacade.UnitOfWork.UserRepository.Value.GetAsync(updateMessage.From!.Id);
        var data =
            await _serviceFacade.ApplicationDataUnitOfWork.UserDataRepository.Value.GetAsync(updateMessage.From!.Id);
        var command = TextCommands.FirstOrDefault(command => command.Compare(updateMessage, user, data));
        if (command != null)
            await command.ExecuteAsync(_botClient, user, data, updateMessage, _serviceFacade);
    }
}