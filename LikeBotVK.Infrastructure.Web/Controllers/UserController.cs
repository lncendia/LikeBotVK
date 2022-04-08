using AutoMapper;
using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Services.WebServices;
using LikeBotVK.Infrastructure.Web.ViewModels.User;
using Microsoft.AspNetCore.Mvc;

namespace LikeBotVK.Infrastructure.Web.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UserController(IUserService userService)
    {
        _userService = userService;
        _mapper = CreateMapper();
    }

    [HttpGet]
    public async Task<IActionResult> Index(UserListViewModel model, string? message = null)
    {
        if (!string.IsNullOrEmpty(message)) ViewData["Alert"] = message;
        model.UserSearchViewModel ??= new UserSearchViewModel();
        if (!ModelState.IsValid) return View(model);
        if (model.UserSearchViewModel.Page < 1) model.UserSearchViewModel.Page = 1;
        var (list, count) = await _userService.FindAsync(model.UserSearchViewModel.UserId,
            model.UserSearchViewModel.Page);
        model.Users = _mapper.Map<List<UserLiteViewModel>>(list);
        model.Count = count;
        if (model.UserSearchViewModel.Page != 1 && !model.Users.Any()) return RedirectToAction("Index");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        var user = await _userService.GetAsync(id);
        if (user == null)
            return RedirectToAction("Index", new {message = "Пользователь не найден."});
        return View(_mapper.Map<UserEditViewModel>(user));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UserEditViewModel userViewModel)
    {
        if (!ModelState.IsValid) return View(userViewModel);
        var dto = _mapper.Map<EditApplicationUserDto>(userViewModel);
        var result = await _userService.EditApplicationDataAsync(dto);

        return RedirectToAction("Index",
            new {message = result ? "Пользователь успешно изменен." : "Пользователь не найден."});
    }

    public async Task<IActionResult> Delete(long id)
    {
        var result = await _userService.DeleteAsync(id);
        return RedirectToAction("Index",
            new {message = result ? "Пользователь успешно удален." : "Пользователь не найден."});
    }

    [HttpGet]
    public IActionResult AddSubscribe(long id)
    {
        var model = new AddSubscribeViewModel {Id = id};
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddSubscribe(AddSubscribeViewModel model)
    {
        if (!ModelState.IsValid)
            return View();
        var result = await _userService.AddSubscribeAsync(model.Id, model.EndSubscribe!.Value);
        return RedirectToAction("Index",
            new {message = result ? "Подписка успешно добавлена." : "Не удалось добавить подписку."});
    }

    private static IMapper CreateMapper()
    {
        return new Mapper(new MapperConfiguration(expr =>
        {
            expr.CreateMap<UserExtendedDto, UserEditViewModel>();
            expr.CreateMap<UserDto, UserLiteViewModel>();
            expr.CreateMap<UserEditViewModel, EditApplicationUserDto>();
        }));
    }
}