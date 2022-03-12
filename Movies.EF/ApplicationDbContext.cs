global using Movies.Core.Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Movies.EF;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options)
    {
    }

    public DbSet<Genre> Genres { get; set; }
    public DbSet<Movie> Movies { get; set; }
}
