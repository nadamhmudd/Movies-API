using Microsoft.AspNetCore.Identity;
using Movies.Core.Constants;
using Movies.Core.DTOs;
using Movies.Core.Interfaces;

namespace Movies.EF.Seeding;
public class DbInitializer : IDbInitializer
{
    private ApplicationDbContext _db;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUnitOfWork _unitOfWork;

    public DbInitializer(
        ApplicationDbContext db,
        RoleManager<IdentityRole> roleManager, 
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _roleManager = roleManager;
        _unitOfWork  = unitOfWork;
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
        _unitOfWork.Auth.RegisterAsync( new RegisterDto
        {  //temprory magic data
            UserName = "Admin",
            Email = "Admin@test.com",
            FirstName = "APP",
            LastName = "Manager",
            Password = "Admin123*"
        });
    }
}
