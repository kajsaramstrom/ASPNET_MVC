using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class AccountController : Controller
{
    //private readonly AccountService _accountService;

    //public AccountController(AccountService accountService)
    //{
    //    _accountService = accountService;
    //}

    [Route("/account")]
    public IActionResult Details()
    {
        var viewModel = new AccountDetailsViewModel();
        //viewModel.BasicInfo = _accountService.GetBasicInfo();
        //viewModel.AddressInfo = _accountService.GetAddressInfo();

        return View(viewModel);
    }

    [Route("/account")]
    [HttpPost]
    public IActionResult BasicInfo(AccountDetailsViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        //_accountService.SaveBasicInfo(viewModel.BasicInfo);
        return RedirectToAction(nameof(Details));
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
