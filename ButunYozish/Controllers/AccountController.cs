using IdentityApi.Data.Context;
using IdentityApi.Data.Entities;
using IdentityApi.Data.Models;
using IdentityApi.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ButunYozish.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
	private readonly IdentityDbContext _context;
	private readonly JwtService _jwtService;

	public AccountController(IdentityDbContext context, JwtService jwtService)
	{
		_context = context;
		_jwtService = jwtService;
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] CreateUser createUser)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest("Notori nimadir boldi");
		}

		var user = new User()
		{
			Username = createUser.Username,
		};

		user.PasswordHash = new PasswordHasher<User>().HashPassword(user, createUser.Password);

		_context.Users.Add(user);
		await _context.SaveChangesAsync();

		return Ok();
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginUserModel loginUserModel)
	{
		var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginUserModel.Username);
		if (user == null)
		{
			return BadRequest("User null");
		}

		var result = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, loginUserModel.Password);

		if (result != PasswordVerificationResult.Success)
		{
			return BadRequest("parol notori");
		}

		var token = _jwtService.GenerateToken(user);

		return Ok(new { Token = token });
	}

	[HttpGet("profile")]
	[Authorize]
	public IActionResult Profile()
	{
		var user = _context.Users.FirstOrDefault(u => u.Id == Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!));
		var userModel = new UserModel()
		{
			Id = user!.Id,
			Username = user.Username,
		};

		return Ok(userModel);
	}

	[HttpGet("GetUser")]
	[Authorize]
	public async Task<User?> GetUser(string username)
	{
		var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

		if (user == null)
		{
			throw new Exception("user topilmadi");
		}

		return user;
	}
}
