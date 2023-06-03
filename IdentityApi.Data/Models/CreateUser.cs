using System.ComponentModel.DataAnnotations;

namespace IdentityApi.Data.Models;

public class CreateUser
{
	public string Username { get; set; }
	public string Password { get; set; }
	[Compare("Password")]
	public string ConfirmPassword { get; set; }
}
