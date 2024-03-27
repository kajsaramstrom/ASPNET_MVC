using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class AboutController(HttpClient httpClient) : Controller
    {
        private readonly HttpClient _httpClient = httpClient;

        [Route("/contact")]
        public IActionResult Contact()
        {
            var viewModel = new ContactViewModel();

            return View(viewModel);
        }

        [HttpPost]
        [Route("/contact")]
        public async Task<IActionResult> Contact(ContactViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var newContactForm = new ContactModel
                    {
                        FullName = viewModel.Contact.FullName,
                        Email = viewModel.Contact.Email,
                        Service = viewModel.Contact.Service,
                        Message = viewModel.Contact.Message
                    };

                    if (newContactForm != null)
                    {
                        var json = JsonConvert.SerializeObject(newContactForm);

                        if (json != null)
                        {
                            var content = new StringContent(json, Encoding.UTF8, "application/json");
                            var response = await _httpClient.PostAsync("https://localhost:7183/api/contact?key=NDA0OTY0ZjQtNjcwNC00ZjIzLWI2MTMtZmRiMDgzOTA5OTQ2", content);

                            if (response.IsSuccessStatusCode)
                            {
                                TempData["Status"] = "You have succesfully sent your message";
                                return RedirectToAction("Contact", "About");
                            }
                        }
                    }
                }
                catch
                {
                    TempData["StatusFail"] = "ConnectionFailed";
                    return RedirectToAction("Contact", "About");
                }
            }
            else 
            {
                TempData["StatusFail"] = "Failed";
                return RedirectToAction("Contact", "About");
            }

            return RedirectToAction("Contact", "About");
        }
    }
}
