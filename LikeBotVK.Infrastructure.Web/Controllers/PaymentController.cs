using AutoMapper;
using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Services.WebServices;
using LikeBotVK.Infrastructure.Web.ViewModels.Payment;
using LikeBotVK.Infrastructure.Web.ViewModels.Proxy;
using Microsoft.AspNetCore.Mvc;

namespace LikeBotVK.Infrastructure.Web.Controllers;

public class PaymentController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly IMapper _mapper;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
        _mapper = GetMapper();
    }

    [HttpGet]
    public async Task<IActionResult> Index(PaymentListViewModel model, string? message = null)
    {
        if (!string.IsNullOrEmpty(message)) ViewData["Alert"] = message;
        model.PaymentSearchViewModel ??= new PaymentSearchViewModel();
        if (!ModelState.IsValid) return View(model);
        if (model.PaymentSearchViewModel.Page < 1) model.PaymentSearchViewModel.Page = 1;
        var (payments, count) =
            await _paymentService.FindAsync(model.PaymentSearchViewModel.UserId, model.PaymentSearchViewModel.Page);

        model.Payments = _mapper.Map<List<PaymentViewModel>>(payments);
        model.Count = count;
        if (model.PaymentSearchViewModel.Page != 1 && !model.Payments.Any()) return RedirectToAction("Index");
        return View(model);
    }

    private static IMapper GetMapper() => new Mapper(new MapperConfiguration(expr => { expr.CreateMap<PaymentDto, PaymentViewModel>(); }));
}