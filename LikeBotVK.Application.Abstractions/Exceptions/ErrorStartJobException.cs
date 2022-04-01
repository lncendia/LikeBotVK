using LikeBotVK.Domain.Jobs.Entities;

namespace LikeBotVK.Application.Abstractions.Exceptions;

public sealed class ErrorStartJobException : Exception
{
    public ErrorStartJobException(Job job, string message, Exception? inner) : base($"Can't start job {job.Id}. {message}.", inner)
    {
    }
}