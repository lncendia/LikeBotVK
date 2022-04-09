namespace LikeBotVK.Application.Abstractions.Exceptions;

public sealed class PublicationsNotFoundException : Exception
{
    public PublicationsNotFoundException(string hashtag) : base($"Can't find any publications with hashtag {hashtag}.")
    {
    }
}