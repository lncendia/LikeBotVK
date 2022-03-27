namespace LikeBotVK.Application.Abstractions.BotServices;

public interface IJobStarterService
{
    Task StartJobAsync(int id, CancellationToken token);
}