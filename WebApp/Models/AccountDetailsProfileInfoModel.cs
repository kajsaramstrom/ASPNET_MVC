namespace WebApp.Models
{
    public class AccountDetailsProfileInfoModel
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string ProfileImageUrl { get; set; } = "image-account-details.svg";
        public bool IsExternalAccount { get; set; }
    }
}
