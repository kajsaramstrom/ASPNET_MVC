using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers;

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
        if (!_signInManager.IsSignedIn(User))
        {
            return RedirectToAction("SignIn", "Auth");
        }

        var viewModel = new AccountDetailsViewModel()
        {
            BasicInfo = await PopulateBasicInfo()
        };

        return View(viewModel);
    }
    #endregion

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

    [Route("/account")]
    [HttpPost]
    public IActionResult AddressInfo(AccountDetailsViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        //_accountService.SaveAddressInfo(viewModel.AddressInfo);
        return RedirectToAction(nameof(Details));
    }


    [Route("/security")]
    public IActionResult Security()
    {
        var viewModel = new AccountSecurityViewModel();

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult Password(AccountDetailsViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        //_accountService.SavePassword(viewModel.Password);
        return RedirectToAction(nameof(Security));
    }

    [HttpPost]
    public IActionResult Delete(AccountDetailsViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        //_accountService.SaveDelete(viewModel.Delete);
        return RedirectToAction(nameof(Security));
    }

    [Route("/saved")]
    public IActionResult SavedCourses()
    {
        var viewModel = new AccountSavedCoursesViewModel();

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult Courses (AccountSavedCoursesViewModel viewModel)
    {
        //_accountService.SavePassword(viewModel.Password);
        return RedirectToAction(nameof(SavedCourses));
    }
}
