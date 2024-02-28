using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class ContactModel
{
    [Display(Name = "Full name", Prompt = "Enter your full name", Order = 0)]
    [Required(ErrorMessage = "Full name is required")]
    [MinLength(2, ErrorMessage = "Enter your fullname")]
    public string FullName { get; set; } = null!;

    [Display(Name = "Email address", Prompt = "Enter your email address", Order = 1)]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Email is required.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Your email address is invalid.")]
    public string Email { get; set; } = null!;

    public string? Service { get; set; }

    [DataType(DataType.MultilineText)]
    [Required(ErrorMessage = "Message is required.")]
    public string Message { get; set; } = null!;
}
