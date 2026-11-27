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

    [HttpGet]
    public async Task<IActionResult> Create(string userId)
    {
        var user = await FindUserAsync(userId);
        if (user == null)
            return RedirectToAction(nameof(Index));

        ViewData["UserName"] = user.UserName;
        return View(new LikeListInput { UserID = user.UserID, Account = user.Account, PurchaseQuantity = 1 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LikeListInput input)
    {
        var user = await FindUserAsync(input.UserID);
        if (user == null)
            return RedirectToAction(nameof(Index));

        ViewData["UserName"] = user.UserName;
        if (!ModelState.IsValid)
            return View(input);

        var result = await _likeListService.CreateAsync(input);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(input);
        }

        TempData["SuccessMessage"] = $"已新增「{input.ProductName}」";
        return RedirectToAction(nameof(Index), new { userId = input.UserID });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int sn, string userId)
    {
        var user = await FindUserAsync(userId);
        if (user == null)
            return RedirectToAction(nameof(Index));

        var likeList = await _likeListService.GetBySNAsync(sn, user.UserID);
        if (likeList == null)
        {
            TempData["ErrorMessage"] = LikeListService.NotFoundMessage;
            return RedirectToAction(nameof(Index), new { userId = user.UserID });
        }

        ViewData["UserName"] = user.UserName;
        return View(new LikeListInput
        {
            SN = likeList.SN,
            UserID = user.UserID,
            ProductName = likeList.ProductName,
            Price = likeList.Price,
            FeeRate = likeList.FeeRate,
            PurchaseQuantity = likeList.PurchaseQuantity,
            Account = likeList.Account
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(LikeListInput input)
    {
        var user = await FindUserAsync(input.UserID);
        if (user == null)
            return RedirectToAction(nameof(Index));

        ViewData["UserName"] = user.UserName;
        if (!ModelState.IsValid)
            return View(input);

        var result = await _likeListService.UpdateAsync(input);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(input);
        }

        TempData["SuccessMessage"] = $"已更新「{input.ProductName}」";
        return RedirectToAction(nameof(Index), new { userId = input.UserID });
    }

    private async Task<User?> FindUserAsync(string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return null;

        var users = await _userService.GetAllAsync();
        return users.FirstOrDefault(u => u.UserID == userId);
    }
}
