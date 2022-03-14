using Microsoft.AspNetCore.Identity;
using Movies.Core.Constants;
using Movies.Core.Interfaces;

namespace Movies.EF.Seeding;
public class DbInitializer : IDbInitializer
{
    private ApplicationDbContext _db;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenHandler _jwt;


    public DbInitializer(
        ApplicationDbContext db,
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager, ITokenHandler jwt)
    {
        _db = db;
        _roleManager = roleManager;
        _userManager = userManager;
        _jwt = jwt;
    }

    public void Initialize()
    {
        //check migrations if they are not applied, no need to updata-database command again
        try
        {
            if (_db.Database.GetPendingMigrations().Count() > 0)
            {
                _db.Database.Migrate();
            }
        }
        catch (Exception)
        {
        }

        //create roles if they are not created
        if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
        {
            _seedRoles();

            //if roles are not created, then we will create admin user as well
           _createAdmin();
        }

        return;
    }

    //----------------Helper Methods------------------------------------
    private void _seedRoles()
    {
        _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
        _roleManager.CreateAsync(new IdentityRole(SD.Role_User)).GetAwaiter().GetResult();
    }

    private void _createAdmin()
    {
        _userManager.CreateAsync(new ApplicationUser
        {
            //temprory magic data
            UserName = "admin",
            Email = "Admin@test.com",
            FirstName = "APP",
            LastName = "Manager",
        }, "Admin123*").GetAwaiter().GetResult();

        ApplicationUser user = _db.Users.FirstOrDefault(u => u.Email == "Admin@test.com");

        _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

        _jwt.CreateJwtToken(user);
    }
}
