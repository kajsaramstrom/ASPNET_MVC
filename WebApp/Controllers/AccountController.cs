using Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize]
public class AccountController(SignInManager<UserEntity> signInManager, UserManager<UserEntity> userManager) : Controller
{
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly UserManager<UserEntity> _userManager = userManager;

    //private readonly AccountService _accountService;

    //public AccountController(AccountService accountService)
    //{
    //    _accountService = accountService;
    //}

    #region Details
    [Route("/account")]
    public async Task<IActionResult> Details()
    {
        var viewModel = new AccountDetailsViewModel();
        //{
        //    BasicInfo = await PopulateBasicInfo()
        //};

        return View(viewModel);
    }
    #endregion

    #region BasicInfo
    [Route("/account")]
    [HttpPost]
    public async Task<IActionResult> BasicInfo(AccountDetailsViewModel viewModel)
    {
        var result = await _userManager.UpdateAsync(viewModel.User);

        if (!result.Succeeded) 
        {
            ModelState.AddModelError("Failed To Save Data", "Unable to save the data.");
            ViewData["ErrorMessage"] = "Unable to save the data.";
        }

        return RedirectToAction("Details", "Account", viewModel);
    }
    #endregion

    #region AddressInfo
    [Route("/account")]
    [HttpPost]
    public IActionResult AddressInfo(AccountDetailsViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        //_accountService.SaveAddressInfo(viewModel.AddressInfo);
        return RedirectToAction(nameof(Details));
    }
    #endregion

    #region Security
    [Route("/security")]
    public IActionResult Security()
    {
        var viewModel = new AccountSecurityViewModel();

        return View(viewModel);
    }
    #endregion

    #region Password
    [HttpPost]
    public IActionResult Password(AccountDetailsViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        //_accountService.SavePassword(viewModel.Password);
        return RedirectToAction(nameof(Security));
    }
    #endregion

    #region Delete
    [HttpPost]
    public IActionResult Delete(AccountDetailsViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        //_accountService.SaveDelete(viewModel.Delete);
        return RedirectToAction(nameof(Security));
    }
    #endregion

    #region Saved
    [Route("/saved")]
    public IActionResult SavedCourses()
    {
        var viewModel = new AccountSavedCoursesViewModel();

        return View(viewModel);
    }
    #endregion

    #region Courses
    [HttpPost]
    public IActionResult Courses (AccountSavedCoursesViewModel viewModel)
    {
        //_accountService.SavePassword(viewModel.Password);
        return RedirectToAction(nameof(SavedCourses));
    }
    #endregion
}
