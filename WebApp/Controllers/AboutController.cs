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
        [Route("/contact")]
        public IActionResult Contact(ContactViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            return RedirectToAction(nameof(Contact));
        }
    }
}
