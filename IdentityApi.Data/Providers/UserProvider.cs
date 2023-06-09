using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityApi.Data.Providers;

public class UserProvider
{
    private readonly IHttpContextAccessor _contextAccessor;

    public UserProvider(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    protected HttpContext? Context => _contextAccessor.HttpContext;

    public string UserName => Context.User.FindFirstValue(ClaimTypes.Name);
    public Guid UserId => Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));
}
