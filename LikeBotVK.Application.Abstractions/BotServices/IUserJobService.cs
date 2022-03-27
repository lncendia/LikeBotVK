using LikeBotVK.Domain.Jobs.Entities;

namespace LikeBotVK.Application.Abstractions.BotServices;

public interface IUserJobService
{
    Task<List<Job>> GetUserNotStartedJobs(long userId);
}