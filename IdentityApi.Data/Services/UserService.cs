using IdentityApi.Data.Context;
using IdentityApi.Data.Entities;
using IdentityApi.Data.Models;
using IdentityApi.Data.Providers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityApi.Data.Services;

public class UserService
{
    private readonly IdentityDbContext _context;
    private readonly JwtService _jwtService;
    private readonly UserProvider _userProvider;
    public UserService(IdentityDbContext context, JwtService jwtService, UserProvider userProvider)
    {
        _context = context;
        _jwtService = jwtService;
        _userProvider = userProvider;
    }

    public async Task<User?> RegisterAsync(CreateUser createUser)
    {

        var user = new User()
        {
            Username = createUser.Username,
        };

        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, createUser.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<string> LoginAsync(LoginUserModel loginUserModel)
    {

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginUserModel.Username);
        if (user == null)
        {
            throw new Exception("User null");
        }

        var result = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, loginUserModel.Password);

        if (result != PasswordVerificationResult.Success)
        {
            throw new Exception("parol muvaffaqiyatsiz heshlandi");
        }

        var token = _jwtService.GenerateToken(user);

        return token;
    }

    public async Task<UserModel> ProfileAsync()
    {
        var userId = _userProvider.UserId;
        var user = await GetUserAsync(userId);

        return new UserModel(user!);
    }

    //bu metod faqat action dan berib yuborish uchun moljallangan
    public async Task<UserModel> GetUserAsync(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            throw new Exception("user topilmadi");
        }

        return new UserModel(user);
    }
    public async Task<User?> GetUserAsync(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            throw new Exception("user topilmadi");
        }

        return user;
    }

}
