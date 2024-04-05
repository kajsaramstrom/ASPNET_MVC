using Infrastructure.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize]
public class AccountController(SignInManager<UserEntity> signInManager, UserManager<UserEntity> userManager, AddressService addressService, HttpClient httpClient) : Controller
{
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly AddressService _addressService = addressService;
    private readonly HttpClient _httpClient = httpClient;

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

                if (String.IsNullOrEmpty(viewModel.Form.CurrentPassword) || String.IsNullOrEmpty(viewModel.Form.NewPassword))
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
    public async Task<IActionResult> SavedCourses()
    {
        var viewmodel = new AccountSavedCoursesViewModel
        {
            BasicInfo = await PopulateBasicInfoAsync(),
            Courses = await PopulateSavedCourses(),
        };

        return View(viewmodel);
    }

    private async Task<IEnumerable<CourseModel>> PopulateSavedCourses()
    {
        string apiUrl = "https://localhost:7183/api/savedcourse/";
        var user = await _userManager.GetUserAsync(User);

        if (user != null)
        {
            var userDto = new UserToGetCoursesModel
            {
                Email = user.Email!
            };

            if (userDto.Email != null)
            {
                var response = await _httpClient.GetAsync($"{apiUrl}{userDto.Email}");

                var json = await response.Content.ReadAsStringAsync();

                var data = JsonConvert.DeserializeObject<IEnumerable<CourseModel>>(json);
                if (data != null)
                {
                    return data;
                }
                else
                {
                    return Enumerable.Empty<CourseModel>();
                }
            }

            return Enumerable.Empty<CourseModel>();
        }

        return Enumerable.Empty<CourseModel>();
    }
    #endregion

    #region Delete Courses
    [HttpPost]
    public async Task<IActionResult> DeleteOneCourse(int courseId)
    {
        string apiUrl = "https://localhost:7183/api/savedcourse/";

        var user = await _userManager.GetUserAsync(User);

        if (user != null)
        {
            var saveCourse = new SaveCourseModel
            {
                UserEmail = user.Email!,
                CourseId = courseId,
            };

            var json = JsonConvert.SerializeObject(saveCourse);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"{apiUrl}{user.Email}"))
            {
                request.Content = content;
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Deleted"] = "Course deleted";
                    return RedirectToAction("SavedCourses", "Account");
                }
                else
                {
                    TempData["Failed"] = "Something went wrong";
                    return RedirectToAction("SavedCourses", "Account");
                }
            }
        }
        return RedirectToAction("SavedCourses", "Account");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAllCourses()
    {
        string apiUrl = "https://localhost:7183/api/savedcourse/";

        var user = await _userManager.GetUserAsync(User);

        if (user != null)
        {
            var saveCourse = new SaveCourseModel
            {
                UserEmail = user.Email!,
            };
            var json = JsonConvert.SerializeObject(saveCourse);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"{apiUrl}{user.Email}/courses"))
            {
                request.Content = content;
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Deleted"] = "All courses deleted";
                    return RedirectToAction("SavedCourses", "Account");
                }
                else
                {
                    TempData["Failed"] = "Something went wrong";
                    return RedirectToAction("SavedCourses", "Account");
                }
            }
        }
        return RedirectToAction("SavedCourses", "Account");
    }
    #endregion

    #region Sub Courses
    [HttpGet]
    public async Task<IActionResult> SubCourse()
    {
        var viewModel = new AccountMyCourseViewModel
        {
            BasicInfo = await PopulateBasicInfoAsync(),
            Courses = await PopulateMyCourses(),
        };
        return View(viewModel);
    }
    #endregion

    private async Task<IEnumerable<CourseModel>> PopulateMyCourses()
    {
        string apiUrl = "https://localhost:7183/api/mycourse/";
        var user = await _userManager.GetUserAsync(User);

        if (user != null)
        {
            var userDto = new UserToGetCoursesModel
            {
                Email = user.Email!
            };
            if (userDto.Email != null)
            {


                var response = await _httpClient.GetAsync($"{apiUrl}{userDto.Email}");

                var json = await response.Content.ReadAsStringAsync();

                var data = JsonConvert.DeserializeObject<IEnumerable<CourseModel>>(json);
                if (data != null)
                {
                    return data;
                }


                else
                {
                    return Enumerable.Empty<CourseModel>();
                }
            }

            return Enumerable.Empty<CourseModel>();
        }
        return Enumerable.Empty<CourseModel>();
    }

    [HttpPost]
    public async Task<IActionResult> SubCourse(int CourseId)
    {
        string apiUrl = "https://localhost:7183/api/mycourse/";

        var user = await _userManager.GetUserAsync(User);

        if (user != null)
        {
            var myCourse = new MyCourseModel
            {
                UserEmail = user.Email!,
                CourseId = CourseId,
            };

            var json = JsonConvert.SerializeObject(myCourse);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var respsonse = await _httpClient.PostAsync(apiUrl, content);

            if (respsonse.IsSuccessStatusCode)
            {
                TempData["Saved"] = "You have now signed up for a course";
                return RedirectToAction("SubCourse", "Account");
            }

            else
            {
                TempData["Failed"] = "Something went wrong";
                return NoContent();
            }
        }

        return BadRequest();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteOneMyCourse(int courseId)
    {
        string apiUrl = "https://localhost:7183/api/mycourse/";

        var user = await _userManager.GetUserAsync(User);

        if (user != null)
        {
            var myCourse = new MyCourseModel
            {
                UserEmail = user.Email!,
                CourseId = courseId,
            };

            var json = JsonConvert.SerializeObject(myCourse);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"{apiUrl}{user.Email}"))
            {
                request.Content = content;
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Deleted"] = "Course deleted";
                    return RedirectToAction("SubCourse", "Account");
                }
                else
                {
                    TempData["Failed"] = "Something went wrong";
                    return RedirectToAction("SubCourse", "Account");
                }
            }
        }
        return RedirectToAction("SubCourse", "Account");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAllMyCourses()
    {
        string apiUrl = "https://localhost:7183/api/mycourse/";

        var user = await _userManager.GetUserAsync(User);

        if (user != null)
        {
            var myCourse = new MyCourseModel
            {
                UserEmail = user.Email!,
            };
            var json = JsonConvert.SerializeObject(myCourse);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"{apiUrl}{user.Email}/courses"))
            {
                request.Content = content;
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Deleted"] = "All courses deleted";
                    return RedirectToAction("SubCourse", "Account");
                }
                else
                {
                    TempData["Failed"] = "Something went wrong";
                    return RedirectToAction("SubCourse", "Account");
                }
            }
        }
        return RedirectToAction("SubCourse", "Account");
    }
}
