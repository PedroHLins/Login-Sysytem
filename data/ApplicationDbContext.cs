using LS.models;
using Microsoft.EntityFrameworkCore;

namespace LS.data;
class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Usuarios { get; set; }
    public DbSet<Admin> admins { get; set; }
}