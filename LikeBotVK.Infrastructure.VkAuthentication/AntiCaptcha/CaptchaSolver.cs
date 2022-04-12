using CaptchaSharp;
using CaptchaSharp.Enums;
using CaptchaSharp.Models;
using CaptchaSharp.Services;
using VkNet.Utils.AntiCaptcha;

namespace LikeBotVK.Infrastructure.VkAuthentication.AntiCaptcha;

public class CaptchaSolver : ICaptchaSolver
{
    private readonly CaptchaService _service;
    private long _id;

    public CaptchaSolver(string token)
    {
        _service = new AntiCaptchaService(token);
    }

    public string Solve(string url)
    {
        using var client = new HttpClient();
        var response = client.Send(new HttpRequestMessage(HttpMethod.Get, url));
        using var ms = new MemoryStream();
        response.Content.ReadAsStream().CopyTo(ms);
        var data = Convert.ToBase64String(ms.GetBuffer());
        var captcha = _service.SolveImageCaptchaAsync(data, new ImageCaptchaOptions
        {
            CaptchaLanguage = CaptchaLanguage.English, MinLength = 4
        }).ConfigureAwait(false).GetAwaiter().GetResult();
        _id = captcha.Id;
        return captcha.Response;
    }

    public void CaptchaIsFalse() => _service.ReportSolution(_id, CaptchaType.ImageCaptcha);
}