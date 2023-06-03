namespace IdentityBlazor.Models;

public class CreateUserModel
{
	public string Password { get; set; }
	public string ConfirmPassword { get; set; }
	public string Username { get; set; }
}
