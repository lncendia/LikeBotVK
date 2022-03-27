using LikeBotVK.Domain.Jobs.Entities;

namespace LikeBotVK.Application.Abstractions.BotServices;

public interface IJobNotifierService
{
     Task NotifyStartAsync(Job job);
     Task NotifyEndAsync(Job job);
}