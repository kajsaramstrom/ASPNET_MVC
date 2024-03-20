using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class HomeController(HttpClient httpClient) : Controller
{
    private readonly HttpClient _httpClient = httpClient;

    public IActionResult Index()
    {
        ViewData["Title"] = "Task Management Assistant You Gonna Love";

        return View();
    }

    [Route("/newsletter")]
    [HttpGet]
    public IActionResult Newsletter()
    {
        var viewModel = new NewsletterViewModel();
        return View(viewModel);
    }

    [Route("/newsletter")]
    [HttpPost]
    public async Task<IActionResult> Newsletter(NewsletterViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var newSub = new NewsletterModel
                {
                    Email = viewModel.Newsletter.Email,
                    DailyNewsletter = viewModel.Newsletter.DailyNewsletter,
                    AdvertisingUpdates = viewModel.Newsletter.AdvertisingUpdates,
                    WeekinReview = viewModel.Newsletter.WeekinReview,
                    StartupsWeekly = viewModel.Newsletter.StartupsWeekly,
                    Podcasts = viewModel.Newsletter.Podcasts,
                    EventUpdates = viewModel.Newsletter.EventUpdates,
                };
                if (newSub != null)
                {
                    var json = JsonConvert.SerializeObject(newSub);

                    if (json != null)
                    {
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await _httpClient.PostAsync("https://localhost:7183/api/subscriber?key=NDA0OTY0ZjQtNjcwNC00ZjIzLWI2MTMtZmRiMDgzOTA5OTQ2", content);

                        if (response.IsSuccessStatusCode)
                        {
                            ViewData["Status"] = "Success";
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                        {
                            ViewData["Status"] = "AlreadyExists";
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            ViewData["Status"] = "Unauthorized";
                        }
                    }
                }
            }
            catch
            {
                ViewData["Status"] = "ConnectionFailed";
            }
        }
        else
        {
            ViewData["Status"] = "Invalid";
        }

        return RedirectToAction("Index");
    }

    [Route("/error")]
    public IActionResult Error()
    {
        ViewData["Title"] = "Page not found";

        return View();
    }
}