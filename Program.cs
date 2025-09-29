using Microsoft.EntityFrameworkCore;
using LS.data;
using Microsoft.Extensions.Options;
using LS.dto;
using LS.models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(Options => Options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddScoped<UserService>();

builder.Services.AddSwaggerGen(options =>
{
    // 1. Adiciona a definição de segurança "Bearer"
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Cabeçalho de autorização JWT usando o esquema Bearer. \r\n\r\n Digite 'Bearer' [espaço] e o seu token no campo abaixo.\r\n\r\nExemplo: \"Bearer 12345abcdef\""
    });

    // 2. Adiciona o requisito de segurança que usa a definição "Bearer"
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/register", async (UserRegisterRequest request, UserService userService) =>
{
    try
    {
        User user = await userService.RegisterUserAsync(request);
        return Results.Ok(new { message = "User sucessfully created! ", userID = user.Id });
    }
    catch (Exception ex)
    {
        return Results.Conflict(ex.Message);
    }
});

app.MapPost("/login", async (UserLoginRequest request, UserService userService) =>
{
    try
    {
        User user = await userService.LoginUserAsync(request);
        var token = userService.GenerateJwtToken(user);
        return Results.Ok(new { token });
    }
    catch (Exception)
    {
        return Results.Unauthorized();
    }
});

app.MapGet("/profile", [Authorize] (ClaimsPrincipal user) => 
{
    var userName = user.Identity!.Name;
    return Results.Ok($"Bem-vindo ao seu perfil, {userName}!");
}).WithName("GetProfile");

app.Run();
