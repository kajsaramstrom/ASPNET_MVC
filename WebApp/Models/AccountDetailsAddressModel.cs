using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class AccountDetailsAddressModel
{
    [Display(Name = "Address line 1", Prompt = "Enter your address line", Order = 0)]
    [Required(ErrorMessage = "Address line is required")]
    [MinLength(2, ErrorMessage = "Enter your address line")]
    public string AddressLineOne { get; set; } = null!;

    [Display(Name = "Address line 2", Prompt = "Enter your second address line", Order = 1)]
    public string? AddressLineTwo { get; set; }

    [Display(Name = "Postal code", Prompt = "Enter your postal code", Order = 2)]
    [DataType(DataType.PostalCode)]
    [Required(ErrorMessage = "Postal code is required")]
    [RegularExpression(@"^\d{3} \d{2}$", ErrorMessage = "Please enter a valid postal code in the format 000 00.")]
    public string PostalCode { get; set; } = null!;

    [Display(Name = "City", Prompt = "Enter your city", Order = 3)]
    [Required(ErrorMessage = "City is required")]
    [MinLength(2, ErrorMessage = "Write the city you live in")]
    public string City { get; set; } = null!;
}
