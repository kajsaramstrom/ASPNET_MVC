using System.ComponentModel.DataAnnotations;
using WebApp.Helpers;

namespace WebApp.Models;

public class NewsletterModel
{
    [Display(Name = "Daily Newsletter", Order = 0)]
    public bool CheckboxOne { get; set; } = false;

    [Display(Name = "Advertising Updates", Order = 1)]
    public bool CheckboxTwo { get; set; } = false;

    [Display(Name = "Week in Review", Order = 2)]
    public bool CheckboxThree { get; set; } = false;

    [Display(Name = "Event Updates", Order = 3)]
    public bool CheckboxFour { get; set; } = false;

    [Display(Name = "Startups Weekly", Order = 4)]
    public bool CheckboxFive { get; set; } = false;

    [Display(Name = "Podcasts", Order = 5)]
    public bool CheckboxSix { get; set; } = false;

    [Display(Prompt = "Your Email", Order = 6)]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Email is required.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Your email address is invalid.")]
    public string Email { get; set; } = null!;
}
