using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class AccountSecurityPasswordModel
{
    [Display(Name = "Current password", Prompt = "********", Order = 0)]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Password is required.")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[\W_]).{8,}$", ErrorMessage = "Invalid password, must be a strong password.")]
    public string CurrentPassword { get; set; } = null!;


    [Display(Name = "New password", Prompt = "********", Order = 1)]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Password is required.")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[\W_]).{8,}$", ErrorMessage = "Invalid password, must be a strong password.")]
    public string NewPassword { get; set; } = null!;


    [Display(Name = "Confirm new password", Prompt = "********", Order = 2)]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Password must be confirmed.")]
    [Compare(nameof(NewPassword), ErrorMessage = "The passwords do not match.")]
    public string ConfirmNewPassword { get; set; } = null!;
}
