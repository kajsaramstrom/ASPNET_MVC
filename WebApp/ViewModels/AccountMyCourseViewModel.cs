using WebApp.Models;

namespace WebApp.ViewModels
{
    public class AccountMyCourseViewModel
    {
        public string Title { get; set; } = "My Courses";

        public CourseModel? Course { get; set; }

        public IEnumerable<CourseModel> Courses { get; set; } = new List<CourseModel>();

        public AccountDetailsBasicInfoModel BasicInfo { get; set; } = new AccountDetailsBasicInfoModel();
    }
}
