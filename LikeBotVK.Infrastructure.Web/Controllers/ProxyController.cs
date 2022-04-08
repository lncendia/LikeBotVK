using AutoMapper;
using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Services.WebServices;
using LikeBotVK.Domain.Proxies.Exceptions;
using LikeBotVK.Infrastructure.Web.ViewModels.Proxy;
using Microsoft.AspNetCore.Mvc;

namespace LikeBotVK.Infrastructure.Web.Controllers;

public class ProxyController : Controller
{
    private readonly IProxyService _proxyService;
    private readonly IMapper _mapper;

    public ProxyController(IProxyService proxyService)
    {
        _proxyService = proxyService;
        _mapper = GetMapper();
    }

    [HttpGet]
    public async Task<IActionResult> Index(ProxyListViewModel model, string? message = null)
    {
        if (!string.IsNullOrEmpty(message)) ViewData["Alert"] = message;
        model.ProxySearchViewModel ??= new ProxySearchViewModel();
        if (!ModelState.IsValid) return View(model);
        if (model.ProxySearchViewModel.Page < 1) model.ProxySearchViewModel.Page = 1;
        var (proxies, count) = await _proxyService.FindAsync(model.ProxySearchViewModel.Host,
            model.ProxySearchViewModel.Port, model.ProxySearchViewModel.Page);
        model.Proxies = _mapper.Map<List<ProxyViewModel>>(proxies);
        model.Count = count;
        if (model.ProxySearchViewModel.Page != 1 && !model.Proxies.Any()) return RedirectToAction("Index");
        return View(model);
    }


    [HttpGet]
    public IActionResult Add()
    {
        var model = new AddProxyViewModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddProxyViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _proxyService.AddProxiesAsync(model.ProxyList);
        }
        catch (InvalidHostFormatException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
        catch (InvalidPortFormatException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }

        return RedirectToAction("Index", new {message = "Прокси успешно загружены."});
    }

    public async Task<IActionResult> Delete(int id)
    {
        var result = await _proxyService.DeleteAsync(id);
        return RedirectToAction("Index", new {message = result ? "Прокси успешно удалена." : "Прокси не найдена."});
    }

    private static IMapper GetMapper() =>
        new Mapper(new MapperConfiguration(expr => { expr.CreateMap<ProxyDto, ProxyViewModel>(); }));
}