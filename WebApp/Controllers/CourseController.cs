using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers;

[Authorize]
public class CourseController(HttpClient httpClient) : Controller
{
    private readonly HttpClient _httpClient = httpClient;

    [HttpGet]
    public async Task<IActionResult> Courses()
    {
        try
        {

            var viewModel = new CourseViewModel();
            viewModel.Courses = await PopulateCourses();
            if (viewModel == null || !viewModel.Courses.Any())
            {
                return NoContent();
            }
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

        string apiUrl = "https://localhost:7183/api/course";


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
}
