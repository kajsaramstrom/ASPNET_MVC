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
                            TempData["Status"] = "Success";
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                        {
                            TempData["Status"] = "AlreadyExists";
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            TempData["Status"] = "Unauthorized";
                        }
                    }
                }
            }
            catch
            {
                TempData["Status"] = "ConnectionFailed";
            }
        }
        else
        {
            TempData["Status"] = "Invalid";
        }

        return RedirectToAction("Index", "Home", null, "subscribe-action");
    }

    [Route("/error")]
    public IActionResult Error()
    {
        ViewData["Title"] = "Page not found";

        return View();
    }

    [HttpGet]

    public IActionResult Unsubscribe()
    {
        var viewModel = new UnSubscribeViewModel();
        return View(viewModel);
    }

    [HttpPost]

    public async Task<IActionResult> Unsubscribe(UnSubscribeViewModel unSubscribe)
    {
        if (ModelState.IsValid)
        {
            if (unSubscribe != null)
            {
                var unSub = new UnSubscribeModel
                {
                    Email = unSubscribe.UnSubscribeModel!.Email,
                    ConfirmBox = unSubscribe.UnSubscribeModel.ConfirmBox,
                };

                string apiKey = "NDA0OTY0ZjQtNjcwNC00ZjIzLWI2MTMtZmRiMDgzOTA5OTQ2";
                string apiUrl = $"https://localhost:7183/api/Subscriber/email?email={unSub.Email}&key={apiKey}";
                var response = await _httpClient.DeleteAsync(apiUrl);


                if (response.IsSuccessStatusCode)
                {
                    TempData["Status"] = "Successfully Unsubscribed";
                    return View(unSubscribe);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["StatusFail"] = "Your Email address was not found ,please try again";
                    return RedirectToAction("Unsubscribe", "Home");
                }
                else
                {
                    TempData["StatusFail"] = "Something went wrong with email or checkbox";
                    return RedirectToAction("Unsubscribe", "Home");
                }
            }
            TempData["StatusFail"] = "You must enter a Email address";
            return RedirectToAction("Unsubscribe", "Home");
        }
        TempData["StatusFail"] = "Checkbox must be checked";
        return RedirectToAction("Unsubscribe", "Home");

    }
}