using System.ComponentModel.DataAnnotations;
using WebApp.Models;

namespace WebApp.ViewModels;

public class SignInViewModel
{
    public string Title { get; set; } = "Sign in";

    public SignInModel Model { get; set; } = new SignInModel();
}
