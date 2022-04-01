using LikeBotVK.Domain.Jobs.Entities;

namespace LikeBotVK.Application.Abstractions.Exceptions;

public sealed class ErrorStopJobException : Exception
{
    public ErrorStopJobException(Job job, string message, Exception? inner) : base($"Can't stop job {job.Id}. {message}.", inner)
    {
    }
}