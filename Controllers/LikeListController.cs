using FPPR_System.Business.Services;
using FPPR_System.Common.Models;
using FPPR_System.Models;
using Microsoft.AspNetCore.Mvc;

namespace FPPR_System.Controllers;

public class LikeListController : Controller
{
    private readonly IUserService _userService;
    private readonly ILikeListService _likeListService;

    public LikeListController(IUserService userService, ILikeListService likeListService)
    {
        _userService = userService;
        _likeListService = likeListService;
    }

    public async Task<IActionResult> Index(string? userId)
    {
        var users = await _userService.GetAllAsync();
        var selectedUser = users.FirstOrDefault(u => u.UserID == userId) ?? users.FirstOrDefault();

        var likeLists = selectedUser == null
            ? Array.Empty<LikeList>()
            : await _likeListService.GetByUserAsync(selectedUser.UserID);

        return View(new LikeListIndexViewModel
        {
            Users = users,
            SelectedUser = selectedUser,
            LikeLists = likeLists
        });
    }
}
