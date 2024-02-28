using WebApp.Models;

namespace WebApp.ViewModels;

public class ContactViewModel
{
    public string Title { get; set; } = "Contact";

    public ContactModel Contact { get; set; } = new ContactModel();
}
