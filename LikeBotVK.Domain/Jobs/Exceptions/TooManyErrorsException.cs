using LikeBotVK.Domain.Jobs.Entities;

namespace LikeBotVK.Domain.Jobs.Exceptions;

public sealed class TooManyErrorsException : Exception
{
    public TooManyErrorsException (Job job) : base($"When processing the job {job.Id}, 10 errors occurred sequentially.")
    {
    }
}