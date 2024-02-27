using System.ComponentModel.DataAnnotations;
using WebApp.Helpers;

namespace WebApp.Models;

public class AccountSecutiryDeleteModel
{
    [Display(Name = "Yes, I want to delete my account.", Order = 1)]
    [CheckboxRequired(ErrorMessage = "You must approve to delete the account.")]
    public bool AcceptDelete { get; set; } = false;
}
