using Infrastructure.Context;
using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Helpers;
using Infrastructure.Models;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Infrastructure.Services;

public class UserService
{
    public readonly UserRepository _repository;
    public readonly AddressService _addressService;
    private readonly DataContext _context;
    private readonly UserManager<UserEntity> _userManager;

    public UserService(UserRepository repository, AddressService addressService, DataContext context, UserManager<UserEntity> userManager)
    {
        _repository = repository;
        _addressService = addressService;
        _context = context;
        _userManager = userManager;
    }

    //public async Task<bool> UpdatedUserAsync(UserEntity user)
    //{
    //    _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

    //    _userManager.Users.FirstOrDefaultAsync(user => user.Email == user.Email);
    //}

    //public async Task<ResponseResult> CreateUserAsync (SignUpViewModel model)
    //{
    //    try
    //    {
    //        var exists = await _repository.AlreadyExistsAsync(x => x.Email == model.Email);

    //        if (exists.StatusCode == StatusCode.EXISTS)
    //            return exists;

    //        var result = await _repository.CreateOneAsync(UserFactory.Create(model));

    //        if (result.StatusCode != StatusCode.OK)
    //            return result;

    //        return ResponseFactory.Ok("User was created successfully.");
    //    }
    //    catch (Exception ex)
    //    {
    //        return ResponseFactory.Error(ex.Message);
    //    }
    //}

    //public async Task<ResponseResult> SignInUserAsync(SignInModel model)
    //{
    //    try
    //    {
    //        var result = await _repository.GetOneAsync(x => x.Email == model.Email);

    //        if (result.StatusCode == StatusCode.OK && result.ContentResult != null)
    //        {
    //            var userEntity = (UserEntity)result.ContentResult;

    //            if (PasswordHasher.ValidateSecurePassword(model.Password, userEntity.SecurityKey, userEntity.Password))
    //                return ResponseFactory.Ok();
    //        }

    //        return ResponseFactory.Error("Incorrect email or password.");
    //    }
    //    catch (Exception ex)
    //    {
    //        return ResponseFactory.Error(ex.Message);
    //    }
    //}
}
