using Infrastructure.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize]
public class AccountController(SignInManager<UserEntity> signInManager, UserManager<UserEntity> userManager, AddressService addressService) : Controller
{
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly AddressService _addressService = addressService;

    #region Details
    [Route("/details")]
    public async Task<IActionResult> Details()
    {
        var viewModel = new AccountDetailsViewModel
        {
            ProfileInfo = await PopulateProfileInfoAsync()
        };

        viewModel.BasicInfo ??= await PopulateBasicInfoAsync();
        viewModel.AddressInfo ??= await PopulateAddressInfoAsync();

        return View(viewModel);
    }
    #endregion

    #region [HttpPost] Details
    [HttpPost]
    [Route("/details")]
    public async Task<IActionResult> Details(AccountDetailsViewModel viewModel)
    {
        if (viewModel.BasicInfo != null)
        {
            if (viewModel.BasicInfo.FirstName != null && viewModel.BasicInfo.LastName != null && viewModel.BasicInfo.Email != null)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    user.FirstName = viewModel.BasicInfo.FirstName;
                    user.LastName = viewModel.BasicInfo.LastName;
                    user.Email = viewModel.BasicInfo.Email;
                    user.UserName = viewModel.BasicInfo.Email;
                    user.PhoneNumber = viewModel.BasicInfo.PhoneNumber;
                    user.Bio = viewModel.BasicInfo.Bio;

                    var result = await _userManager.UpdateAsync(user);

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("IncorrectValues", "Something went wrong! Unable to save data.");
                        ViewData["ErrorMessage"] = "Something went wrong! Unable to update basic information.";
                    }
                }
            }
        }

        if (viewModel.AddressInfo != null)
        {
            if (viewModel.AddressInfo.AddressLineOne != null && viewModel.AddressInfo.PostalCode != null && viewModel.AddressInfo.City != null)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    var address = await _addressService.GetAddressAsync(user.Id);

                    if (address != null)
                    {
                        address.AddressLine_1 = viewModel.AddressInfo.AddressLineOne;
                        address.AddressLine_2 = viewModel.AddressInfo.AddressLineTwo;
                        address.PostalCode = viewModel.AddressInfo.PostalCode;
                        address.City = viewModel.AddressInfo.City;

                        var result = await _addressService.UpdateAddressAsync(address);
                        if (!result)
                        {
                            ModelState.AddModelError("IncorrectValues", "Something went wrong! Unable to update address information.");
                            ViewData["ErrorMessage"] = "Something went wrong! Unable to update address information.";
                        }
                    }
                    else
                    {
                        address = new AddressEntity
                        {
                            UserId = user.Id,
                            AddressLine_1 = viewModel.AddressInfo.AddressLineOne,
                            AddressLine_2 = viewModel.AddressInfo.AddressLineTwo,
                            PostalCode = viewModel.AddressInfo.PostalCode,
                            City = viewModel.AddressInfo.City
                        };

                        var result = await _addressService.CreateAddressAsync(address);
                        if (!result)
                        {
                            ModelState.AddModelError("IncorrectValues", "Something went wrong! Unable to update address information.");
                            ViewData["ErrorMessage"] = "Something went wrong! Unable to update address information.";
                        }
                    }
                }
            }
        }

        viewModel.ProfileInfo = await PopulateProfileInfoAsync();
        viewModel.BasicInfo ??= await PopulateBasicInfoAsync();
        viewModel.AddressInfo ??= await PopulateAddressInfoAsync();

        return View(viewModel);
    }
    #endregion

    private async Task<AccountDetailsProfileInfoModel> PopulateProfileInfoAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        return new AccountDetailsProfileInfoModel
        {
            FirstName = user!.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            IsExternalAccount = user.IsExternalAccount,
        };
    }

    private async Task<AccountDetailsBasicInfoModel> PopulateBasicInfoAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        return new AccountDetailsBasicInfoModel
        {
            UserId = user!.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber,
            Bio = user.Bio
        };
    }

    private async Task<AccountDetailsAddressModel> PopulateAddressInfoAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            var address = await _addressService.GetAddressAsync(user.Id);

            if (address != null)
            {
                return new AccountDetailsAddressModel
                {
                    AddressLineOne = address.AddressLine_1,
                    AddressLineTwo = address.AddressLine_2,
                    PostalCode = address.PostalCode,
                    City = address.City
                };
            }
        }

        return new AccountDetailsAddressModel();
    }

    #region Security
    [Route("/security")]
    public async Task<IActionResult> Security()
    {
        var viewModel = new AccountSecurityViewModel()
        {
            BasicInfo = await PopulateBasicInfoAsync()
        };

        return View(viewModel);
    }
    #endregion

    #region Password
    [HttpPost]
    public async Task<IActionResult> Password(AccountSecurityViewModel viewModel)
    {
        if (viewModel.Form != null)
        {
            var userEntity = await _userManager.GetUserAsync(User);

            if (userEntity != null)
            {

                if (String.IsNullOrEmpty(viewModel.Form.CurrentPassword) || !String.IsNullOrEmpty(viewModel.Form.NewPassword))
                {
                    TempData["PasswordError"] = "Something went wrong, please check your passwords.";
                    return RedirectToAction("Security", "Account");
                }

                    var passwordChange = await _userManager.ChangePasswordAsync(userEntity, viewModel.Form.CurrentPassword, viewModel.Form.NewPassword);
                if (passwordChange.Succeeded)
                {
                    var result = await _userManager.UpdateAsync(userEntity);
                    if (result.Succeeded)
                    {
                        TempData["PasswordSuccess"] = "Password was successfully changed.";
                        return RedirectToAction("Security", "Account");
                    }
                }

                TempData["PasswordError"] = "Something went wrong, please check your passwords.";
                return RedirectToAction("Security", "Account");
            }
        }

        return RedirectToAction("Security", "Account");
    }
    #endregion

    #region Delete
    [HttpPost]
    public async Task<IActionResult> Delete(AccountSecurityViewModel viewModel)
    {
        if (viewModel.Delete != null)
        {
            var userEntity = await _userManager.GetUserAsync(User);

            if (userEntity != null)
            {

                if (viewModel.Delete.AcceptDelete ==  true)
                {
                    var address = await _addressService.RemoveAddressAsync(userEntity.Id);
                    var result = await _userManager.DeleteAsync(userEntity);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignOutAsync();
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    TempData["PasswordError"] = "Checkbox must be confirmed.";
                    return RedirectToAction("Security", "Account");
                }
            }
        }

        return RedirectToAction("Security", "Account");
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
    public IActionResult Courses(AccountSavedCoursesViewModel viewModel)
    {
        //_accountService.SavePassword(viewModel.Password);
        return RedirectToAction(nameof(SavedCourses));
    }
    #endregion
}
