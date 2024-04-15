using WebApp.Models;

namespace WebApp.ViewModels;

public class CourseViewModel
{
    public string Title { get; set; } = "Courses";
    public CourseModel? Course { get; set; }
    public PaginationModel Pagination { get; set; } = new PaginationModel();
    public IEnumerable<CourseModel> Courses { get; set; } = new List<CourseModel>();
    public IEnumerable<CourseModel> SavedCourses { get; set; } = new List<CourseModel>();
}
