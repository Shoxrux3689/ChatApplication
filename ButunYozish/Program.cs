using ButunYozish.Hubs;
using ChatData.Context;
using IdentityApi.Data.Context;
using IdentityApi.Data.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
	{
		Description = "JWT Bearer. : \"Authorization: Bearer { token }\"",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new string[]{}
		}
	});
});

builder.Services.AddDbContext<IdentityDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDb"));
});
builder.Services.AddDbContext<ChatDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDb"));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
	var signingKey = System.Text.Encoding.UTF32.GetBytes(builder.Configuration.GetSection("JwtOptions:SignIngKey").Value);

	options.TokenValidationParameters = new TokenValidationParameters()
	{
		ValidIssuer = builder.Configuration.GetSection("JwtOptions:ValidIssuer").Value,
		ValidAudience = builder.Configuration.GetSection("JwtOptions:ValidAudience").Value,
		ValidateIssuer = true,
		ValidateAudience = true,
		IssuerSigningKey = new SymmetricSecurityKey(signingKey),
		ValidateIssuerSigningKey = true,
		ValidateLifetime = true,
		ClockSkew = TimeSpan.Zero
	};

	options.Events = new JwtBearerEvents()
	{
		OnMessageReceived = async context =>
		{
			if (string.IsNullOrEmpty(context.Token))
			{
				//agar requestni headerida 'Authorization'
				//nomi bilan token junatilmasa
				//tokenni requestni querysidan
				//olish

				//barcha routelar uchun
				var accessToken = context.Request.Query["token"];
				context.Token = accessToken;

				// faqat 'hubs' bilan boshlangan routelar uchun
				/*var accessToken = context.Request.Query["access_token"];
				var path = context.HttpContext.Request.Path;
				if (!string.IsNullOrEmpty(accessToken) &&
					path.StartsWithSegments("/hubs"))
				{
					context.Token = accessToken;
				}*/
			}
		}
	};
});

builder.Services.AddScoped<JwtService>();

//signalr ishlashi uchun qoshish kerak
builder.Services.AddSignalR();
//httpcontext ga controllerdan tashqarida boglanish uchun 
builder.Services.AddHttpContextAccessor();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(cors =>
{
	cors.AllowAnyHeader()
		.AllowAnyMethod()
		.AllowAnyOrigin();
});

/*
if (app.Services.GetService<IdentityDbContext>() != null)
{
    var chatDb = app.Services.GetRequiredService<IdentityDbContext>();
    chatDb.Database.Migrate();
}
*/

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<ChatHub>("/hubs/chatuchun");

app.MapControllers();

app.Run();
