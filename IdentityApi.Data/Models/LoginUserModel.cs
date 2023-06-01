using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityApi.Data.Models;

public class LoginUserModel
{
	public string Password { get; set; }
	public string Username { get; set; }
}
