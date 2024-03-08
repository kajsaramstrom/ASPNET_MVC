using Infrastructure.Entities;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class AuthController(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager) : Controller
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;

    #region Sign Up
    [HttpGet]
    [Route("/signup")]
    public IActionResult SignUp()
    {
        var viewModel = new SignUpViewModel();

        if (_signInManager.IsSignedIn(User))
            return RedirectToAction("Details", "Account");

        return View(viewModel);
    }

    [Route("/signup")]
    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var exists = await _userManager.Users.AnyAsync(x => x.Email == viewModel.Model.Email);

            if (exists)
            {
                ModelState.AddModelError("AlreadyExists", "User with the same email address already exists.");
                ViewData["ErrorMessage"] = "User with the same email address already exists.";
                return View(viewModel);
            }

            var userEntity = new UserEntity
            {
                FirstName = viewModel.Model.FirstName,
                LastName = viewModel.Model.LastName,
                Email = viewModel.Model.Email,
                UserName = viewModel.Model.Email
            };

            var result = await _userManager.CreateAsync(userEntity, viewModel.Model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("SignIn", "Auth");
            }
        }

        return View(viewModel);
    }
    #endregion

    #region Sign In
    [Route("/signin")]
    [HttpGet]
    public IActionResult SignIn(string returnUrl)
    {
        var viewModel = new SignInViewModel();

        if (_signInManager.IsSignedIn(User))
            return RedirectToAction("Details", "Account");

        ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");

        return View(viewModel);
    }

    [Route("/signin")]
    [HttpPost]
    public async Task<IActionResult> SignIn(SignInViewModel viewModel, string returnUrl)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, viewModel.RememberMe, false);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Details", "Account");
            }
        }

        ModelState.AddModelError("IncorrectValues", "Incorrect email or password.");
        ViewData["ErrorMessage"] = "Incorrect email or password";
        return View(viewModel);
    }
    #endregion

    #region Sign Out
    [HttpGet]
    [Route("/signout")]
    public new async Task<IActionResult> SignOut()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }
    #endregion

}
