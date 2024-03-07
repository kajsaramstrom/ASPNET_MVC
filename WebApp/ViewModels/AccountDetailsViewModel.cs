using Infrastructure.Entities;
using WebApp.Models;

namespace WebApp.ViewModels;

public class AccountDetailsViewModel
{
    public string Title { get; set; } = "Account Details";

    public AccountDetailsProfileInfoModel? ProfileInfo { get; set; }

    public AccountDetailsBasicInfoModel? BasicInfo { get; set; }

    public AccountDetailsAddressModel? AddressInfo { get; set; }
}