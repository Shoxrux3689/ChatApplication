using IdentityApi.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityApi.Data.Services;

public class JwtService
{
    private readonly IConfiguration configuration;

    public JwtService(IConfiguration _configuration)
    {
        configuration = _configuration;
    }
    public string GenerateToken(User user)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
        };

        var signingKey = Encoding.UTF32.GetBytes(configuration.GetSection("JwtOptions:SignIngKey").Value);
        var security = new JwtSecurityToken(
            issuer: configuration.GetSection("JwtOptions:ValidIssuer").Value,
            audience: configuration.GetSection("JwtOptions:ValidAudience").Value,
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToInt64(configuration.GetSection("JwtOptions:ExpiresMinutes").Value)),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256)
            );

        var token = new JwtSecurityTokenHandler().WriteToken(security);

        return token;
    }
}
