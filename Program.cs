using Microsoft.EntityFrameworkCore;
using LS.data;
using Microsoft.Extensions.Options;
using LS.dto;
using LS.models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(Options => Options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddScoped<UserService>();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


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
        return Results.Ok("Sucessfully login");
    }
    catch (Exception)
    {
        return Results.Unauthorized();
    }
});

app.Run();
