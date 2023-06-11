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
	private readonly UserService _userService;
	public AccountController(UserService userService)
	{
		_userService = userService;
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] CreateUser createUser)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest("Notori nimadir boldi");
		}

		var user = await _userService.RegisterAsync(createUser);

		return Ok();
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginUserModel loginUserModel)
	{
        if (!ModelState.IsValid)
        {
            return BadRequest("Notori nimadir boldi");
        }

        return Ok(new { Token = await _userService.LoginAsync(loginUserModel) });
    }
	

	[HttpGet("profile")]
	[Authorize]
	public async Task<IActionResult> Profile()
	{
		var userModel = await _userService.ProfileAsync();

		return Ok(userModel);
	}

	[HttpGet("getuser")]
	[Authorize]
	public async Task<UserModel?> GetUser(string username)
	{
		return await _userService.GetUserAsync(username);
	}
}
