using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class AboutController : Controller
    {
        [Route("/contact")]
        public IActionResult Contact()
        {
            var viewModel = new ContactViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel viewModel)
        {
            return RedirectToAction(nameof(Contact));
        }
    }
}
