using LikeBotVK.Domain.Jobs.Entities;

namespace LikeBotVK.Domain.Jobs.Exceptions;

public sealed class ErrorStartJobException : Exception
{
    public ErrorStartJobException(Job job, string message, Exception? inner) : base($"Can't start job {job.Id}. {message}.", inner)
    {
    }
}