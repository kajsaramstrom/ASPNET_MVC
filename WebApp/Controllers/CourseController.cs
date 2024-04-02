using Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize]
public class CourseController(HttpClient httpClient, UserManager<UserEntity> userManager) : Controller
{
    private readonly HttpClient _httpClient = httpClient;
    private UserManager<UserEntity> _userManager = userManager;

    #region SAVE COURSE
    [HttpPost]
    public async Task<IActionResult> SaveCourse(int CourseId)
    {
        string apiUrl = "https://localhost:7183/api/SavedCourse";

        var user = await _userManager.GetUserAsync(User);

        if (user != null)
        {
            var saveCourse = new SaveCourseModel
            {
                UserEmail = user.Email!,
                CourseId = CourseId,
            };

            var json = JsonConvert.SerializeObject(saveCourse);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var respsonse = await _httpClient.PostAsync(apiUrl, content);

            if (respsonse.IsSuccessStatusCode)
            {
                TempData["Saved"] = "Course saved";
                return Ok(respsonse);
            }
            else
            {
                TempData["Failed"] = "Something went wrong";
                return NoContent();
            }
        }

        return BadRequest();
    }
    #endregion

    #region GET COURSE
    [HttpGet]
    public async Task<IActionResult> Courses(string searchString, int? category, int pageNumber = 1, int pageSize = 6)
    {
        try
        {
            var viewModel = new CourseViewModel();
            viewModel.Courses = await PopulateCourses();

            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                viewModel.Courses = viewModel.Courses.Where(s =>
                    s.Title.ToLower().Contains(searchString) ||
                    s.Author!.ToLower().Contains(searchString));
            }

            if (category.HasValue)
            {
                switch (category)
                {
                    case 1:
                        viewModel.Courses = viewModel.Courses.Where(c => c.IsBestSeller == true);
                        break;
                    case 2:
                        viewModel.Courses = viewModel.Courses.Where(c => c.DiscountPrice != null);
                        break;
                    default:
                        break;
                }
            }

            viewModel.Pagination.CurrentPage = pageNumber;
            viewModel.Pagination.PageSize = pageSize;
            viewModel.Pagination.TotalItems = viewModel.Courses.Count();

            viewModel.Courses = viewModel.Courses.Skip((pageNumber - 1) * pageSize)
                                                 .Take(pageSize)
                                                 .ToList();

            return View(viewModel);
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<IEnumerable<CourseModel>> PopulateCourses()
    {

        string apiUrl = "https://localhost:7183/api/course?key=NDA0OTY0ZjQtNjcwNC00ZjIzLWI2MTMtZmRiMDgzOTA5OTQ2";


        var response = await _httpClient.GetAsync(apiUrl);

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
    #endregion

    #region SINGLE COURSE
    [HttpGet]
    public async Task<IActionResult> SingleCourse(int id)
    {
        string apiUrl = "https://localhost:7183/api/course/" + id;

        var response = await _httpClient.GetAsync(apiUrl);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var courseModel = JsonConvert.DeserializeObject<CourseModel>(json);

            if (courseModel != null)
            {
                var viewModel = new CourseViewModel
                {
                    Course = courseModel
                };
                return View(viewModel);
            }
        }

        return RedirectToAction("Index", "Course");
    }
    #endregion
}
