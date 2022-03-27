namespace LikeBotVK.Application.Abstractions.Extensions;

public static class StringExtension
{
    public static string ToHtmlStyle(this string text) =>
        text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
}