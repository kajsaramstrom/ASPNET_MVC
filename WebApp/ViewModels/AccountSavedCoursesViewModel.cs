using WebApp.Models;

namespace WebApp.ViewModels;

public class AccountSavedCoursesViewModel
{
    public string Title { get; set; } = "Account Saved Courses";

    public CoursesModel Courses { get; set; } = new CoursesModel();

    public AccountDetailsBasicInfoModel BasicInfo { get; set; } = new AccountDetailsBasicInfoModel()
    {
        ProfileImage = "images/images/image-account-details.svg",
        FirstName = "John",
        LastName = "Doe",
        Email = "john.doe@domain.com"
    };
}
