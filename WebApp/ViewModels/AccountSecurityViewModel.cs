using WebApp.Models;

namespace WebApp.ViewModels;

public class AccountSecurityViewModel
{
    public string Title { get; set; } = "Account Security";

    public AccountSecurityPasswordModel Form { get; set; } = new AccountSecurityPasswordModel();

    public AccountSecutiryDeleteModel Delete { get; set; } = new AccountSecutiryDeleteModel();

    public AccountDetailsBasicInfoModel BasicInfo { get; set; } = new AccountDetailsBasicInfoModel();
}
