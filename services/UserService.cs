using LS.data;
using LS.dto;
using LS.models;
using Microsoft.EntityFrameworkCore;

class UserService
{
    private readonly ApplicationDbContext _db;

    public UserService(ApplicationDbContext db)
    {
        _db = db;
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
}