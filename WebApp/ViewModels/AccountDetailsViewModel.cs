using WebApp.Models;

namespace WebApp.ViewModels;

public class AccountDetailsViewModel
{
    public string Title { get; set; } = "Account Details";

    public AccountDetailsBasicInfoModel BasicInfo { get; set; } = new AccountDetailsBasicInfoModel()
    {
        ProfileImage = "images/images/image-account-details.svg",
        FirstName = "John",
        LastName = "Doe",
        Email = "john.doe@domain.com"
    };

    public AccountDetailsAddressModel AddressInfo { get; set; } = new AccountDetailsAddressModel();
}
