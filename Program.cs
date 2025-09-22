using Microsoft.EntityFrameworkCore;
using LS.data;
using Microsoft.Extensions.Options;
using LS.dto;
using LS.models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(Options => Options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/register", (UserRegisterRequest user, ApplicationDbContext db) =>
{
    string userPassword = user.Password!;

    string passwordHashed = BCrypt.Net.BCrypt.HashPassword(userPassword);

    User newUser = new User
    {
        Name = user.Name!,
        Email = user.Email!,
        PasswordHash = passwordHashed!
    };

    db.Usuarios.Add(newUser);
    db.SaveChanges();

    return Results.Ok("User sucessfully created!");
});

app.Run();
