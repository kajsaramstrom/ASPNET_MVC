using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations;
using WebApp.Models;

namespace WebApp.ViewModels;

public class SignUpViewModel
{
    public string Title { get; set; } = "Sign up";

    public SignUpModel Model { get; set; } = new SignUpModel();
}
