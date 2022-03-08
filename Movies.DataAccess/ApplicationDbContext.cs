using Microsoft.EntityFrameworkCore;
using Movies.Models;

namespace Movies.DataAccess;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options)
    {
    }

    public DbSet<Genre> Genres { get; set; }
}
