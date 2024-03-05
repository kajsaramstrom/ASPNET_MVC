using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
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
        public IActionResult Newsletter(NewsletterViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View("Index", viewModel);

            // _newsletterService.CreateNewsletter(Newsletter);

            return RedirectToAction("Index");
        }

        [Route("/error")]
        public IActionResult Error()
        {
            ViewData["Title"] = "Page not found";

            return View();
        }
    }
}