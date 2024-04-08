using Infrastructure.Entities;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class AuthController(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, HttpClient httpClient) : Controller
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly HttpClient _httpClient = httpClient;


    #region Individual account - Sign Up
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

    #region Individual account - Sign In
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
            var result = await _signInManager.PasswordSignInAsync(viewModel.Model.Email, viewModel.Model.Password, viewModel.Model.RememberMe, false);
            if (result.Succeeded)
            {
                var userDto = new ApiUserModel
                {
                    Email = viewModel.Model.Email,
                };

                var json = JsonConvert.SerializeObject(userDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://localhost:7183/api/Auth?key=NDA0OTY0ZjQtNjcwNC00ZjIzLWI2MTMtZmRiMDgzOTA5OTQ2", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);

                    if (tokenResponse != null && tokenResponse.TryGetValue("token", out var token) && !string.IsNullOrEmpty(token))
                    {

                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            Expires = DateTime.UtcNow.AddDays(1)
                        };

                        Response.Cookies.Append("AccessToken", token, cookieOptions);

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                            return Redirect(returnUrl);

                        return RedirectToAction("Details", "Account");
                    }

                    return Unauthorized("Novalid token found");
                }
            }
        }

        ModelState.AddModelError("IncorrectValues", "Incorrect email or password.");
        ViewData["ErrorMessage"] = "Incorrect email or password";
        return View(viewModel);
    }
    #endregion

    #region Individual account - Sign Out
    [HttpGet]
    [Route("/signout")]
    public new async Task<IActionResult> SignOut()
    {
        Response.Cookies.Delete("AccessToken");
        await _signInManager.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }
    #endregion

    #region External account - Facebook

    [HttpGet]
    public IActionResult Facebook()
    {
        var authProps = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", Url.Action("FacebookCallback"));
        return new ChallengeResult("Facebook", authProps);
    }

    [HttpGet]
    public async Task<IActionResult> FacebookCallback()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info != null)
        {
            var userEntity = new UserEntity
            {
                FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName)!,
                LastName = info.Principal.FindFirstValue(ClaimTypes.Surname)!,
                Email = info.Principal.FindFirstValue(ClaimTypes.Email)!,
                UserName = info.Principal.FindFirstValue(ClaimTypes.Email)!,
                IsExternalAccount = true
            };

            var user = await _userManager.FindByEmailAsync(userEntity.Email);
            if (user == null)
            {
                var result = await _userManager.CreateAsync(userEntity);
                if (result.Succeeded)
                    user = await _userManager.FindByEmailAsync(userEntity.Email);
            }

            if (user != null)
            {
                if (user.FirstName != userEntity.FirstName || user.LastName != userEntity.LastName || user.Email != userEntity.Email)
                {
                    user.FirstName = userEntity.FirstName;
                    user.LastName = userEntity.LastName;
                    user.Email = userEntity.Email;

                    await _userManager.UpdateAsync(user);
                }

                await _signInManager.SignInAsync(user, isPersistent: false);

                if (HttpContext.User != null)
                {
                    var userDto = new ApiUserModel
                    {
                        Email = user.Email,
                    };

                    var json = JsonConvert.SerializeObject(userDto);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync("https://localhost:7183/api/Auth?key=NDA0OTY0ZjQtNjcwNC00ZjIzLWI2MTMtZmRiMDgzOTA5OTQ2", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var tokenResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);

                        if (tokenResponse != null && tokenResponse.TryGetValue("token", out var token) && !string.IsNullOrEmpty(token))
                        {

                            var cookieOptions = new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                Expires = DateTime.UtcNow.AddDays(1)
                            };

                            Response.Cookies.Append("AccessToken", token, cookieOptions);
                            return RedirectToAction("Details", "Account");
                        }
                    }
                }

                ModelState.AddModelError("InvalidFacebookAuthentication", "Failed to authentication with Faccebook.");
                ViewData["StatusMessage"] = "Failed to authentication with Facebook.";
                return RedirectToAction("SignIn", "Auth");
            }
        }

        return RedirectToAction("SignIn", "Auth");
    }
    #endregion

    #region External account - Google

    [HttpGet]
    public IActionResult Google()
    {
        var authProps = _signInManager.ConfigureExternalAuthenticationProperties("Google", Url.Action("GoogleCallback"));
        return new ChallengeResult("Google", authProps);
    }

    [HttpGet]
    public async Task<IActionResult> GoogleCallback()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info != null)
        {
            var userEntity = new UserEntity
            {
                FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName)!,
                LastName = info.Principal.FindFirstValue(ClaimTypes.Surname)!,
                Email = info.Principal.FindFirstValue(ClaimTypes.Email)!,
                UserName = info.Principal.FindFirstValue(ClaimTypes.Email)!,
                IsExternalAccount = true
            };

            var user = await _userManager.FindByEmailAsync(userEntity.Email);
            if (user == null)
            {
                var result = await _userManager.CreateAsync(userEntity);
                if (result.Succeeded)
                    user = await _userManager.FindByEmailAsync(userEntity.Email);
            }

            if (user != null)
            {
                if (user.FirstName != userEntity.FirstName || user.LastName != userEntity.LastName || user.Email != userEntity.Email)
                {
                    user.FirstName = userEntity.FirstName;
                    user.LastName = userEntity.LastName;
                    user.Email = userEntity.Email;

                    await _userManager.UpdateAsync(user);
                }

                await _signInManager.SignInAsync(user, isPersistent: false);

                if (HttpContext.User != null)
                {
                    var userDto = new ApiUserModel
                    {
                        Email = user.Email,
                    };

                    var json = JsonConvert.SerializeObject(userDto);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync("https://localhost:7183/api/Auth?key=NDA0OTY0ZjQtNjcwNC00ZjIzLWI2MTMtZmRiMDgzOTA5OTQ2", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var tokenResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);

                        if (tokenResponse != null && tokenResponse.TryGetValue("token", out var token) && !string.IsNullOrEmpty(token))
                        {

                            var cookieOptions = new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                Expires = DateTime.UtcNow.AddDays(1)
                            };

                            Response.Cookies.Append("AccessToken", token, cookieOptions);
                            return RedirectToAction("Details", "Account");
                        }
                    }
                }
            }
        }

        ModelState.AddModelError("InvalidFacebookAuthentication", "Failed to authentication with Google.");
        ViewData["StatusMessage"] = "Failed to authentication with Google.";
        return RedirectToAction("SignIn", "Auth");
    }
    #endregion
}
