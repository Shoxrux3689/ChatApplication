using ButunYozish.Hubs;
using ChatData.Context;
using IdentityApi.Data.Context;
using IdentityApi.Data.Services;
using IdentityApi.Data.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
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


//signalr ishlashi uchun qoshish kerak
builder.Services.AddSignalR();
//httpcontext ga controllerdan tashqarida boglanish uchun 
builder.Services.AddHttpContextAccessor();

builder.Services.AddIdentity(builder.Configuration);

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(cors =>
{
	cors.AllowAnyHeader()
		.AllowAnyMethod()
		.AllowAnyOrigin();
});



app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<ChatHub>("/hubs/chatuchun");

app.MapControllers();

app.Run();
