namespace WebApp.Models;

public class CoursesModel
{
    public string Image { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Author {  get; set; } = null!;
    public int price { get; set; } = 0;
    public string DateTime { get; set; } = null!;
    public string Like { get; set; } = null!;
}
