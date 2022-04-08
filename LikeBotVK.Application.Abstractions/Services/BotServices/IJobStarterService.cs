namespace LikeBotVK.Application.Abstractions.Services.BotServices;

public interface IJobStarterService
{
    Task StartJobAsync(int id, CancellationToken token);
}