using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LS.data;
using LS.dto;
using LS.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

class UserService
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _configuration;

    public UserService(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<User> RegisterUserAsync(UserRegisterRequest request)
    {

        var userExists = await _db.Usuarios.AnyAsync(u => u.Email == request.Email);
        if (userExists)
        {
            throw new Exception("Another user already use this email adress");
        }

        string userPassword = request.Password!;
        string passwordHashed = BCrypt.Net.BCrypt.HashPassword(userPassword);

        User newUser = new User
        {
            Name = request.Name!,
            Email = request.Email!,
            PasswordHash = passwordHashed!
        };

        _db.Usuarios.Add(newUser);
        await _db.SaveChangesAsync();

        return newUser;
    }

    public async Task<User> LoginUserAsync(UserLoginRequest request)
    {
        var user = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || request.Password == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new Exception("Invalid email or password");
        }

        return user;
    }

    public string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, "User"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}