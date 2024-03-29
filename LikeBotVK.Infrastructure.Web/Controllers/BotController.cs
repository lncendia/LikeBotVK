using LikeBotVK.Application.Abstractions.Services.BotServices;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace LikeBotVK.Infrastructure.Web.Controllers;

public class BotController : ControllerBase
{
    private readonly IUpdateHandler _updateService;

    public BotController(IUpdateHandler updateService)
    {
        _updateService = updateService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
        await _updateService.HandleAsync(update);
        return Ok();
    }
}