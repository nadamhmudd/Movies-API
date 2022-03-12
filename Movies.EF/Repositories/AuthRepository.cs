using Microsoft.AspNetCore.Identity;
using Movies.Core.appsettings;
using Movies.Core.Interfaces;

namespace Movies.EF.Repositories;
public class AuthRepository : IAuthRepository
{
    private UserManager<ApplicationUser> userManager;
    private RoleManager<IdentityRole> roleManager;
    private JWT jwt;

    public AuthRepository(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager, 
        JWT jwt)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.jwt = jwt;
    }
}
