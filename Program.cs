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

app.MapPost("/register", (UserRegisterRequest request, ApplicationDbContext db) =>
{
    string userPassword = request.Password!;

    string passwordHashed = BCrypt.Net.BCrypt.HashPassword(userPassword);

    User newUser = new User
    {
        Name = request.Name!,
        Email = request.Email!,
        PasswordHash = passwordHashed!
    };

    db.Usuarios.Add(newUser);
    db.SaveChanges();

    return Results.Ok("User sucessfully created!");
});

app.MapPost("/login", async (UserLoginRequest request, ApplicationDbContext db) =>
{
    var user = await db.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);

    if (request.Password == null || !BCrypt.Net.BCrypt.Verify(request.Password, user!.PasswordHash))
    {
        return Results.Unauthorized();
    }

    return Results.Ok("Sucessfully login");
});

app.Run();
