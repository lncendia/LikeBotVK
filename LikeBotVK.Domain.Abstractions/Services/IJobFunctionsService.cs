using LikeBotVK.Domain.Jobs.ValueObjects;
using LikeBotVK.Domain.VK.Entities;

namespace LikeBotVK.Domain.Abstractions.Services;

public interface IJobFunctionsService
{
    Task LikeAsync(Vk vk, Publication publication);
    Task RepostAsync(Vk vk, Publication publication);
    Task FollowAsync(Vk vk, Publication publication);
}