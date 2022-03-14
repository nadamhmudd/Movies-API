using Microsoft.AspNetCore.Identity;
using Movies.Core.Entities.Models;
using Movies.Core.Interfaces;
using Movies.EF;
using Movies.EF.Repositories;
using Movies.Core.appsettings;
using AutoMapper;
using Microsoft.Extensions.Options;

namespace Movies.Services.UOW;

public class UnitOfWork : IUnitOfWork
{
    //The only place can access database 
    private readonly ApplicationDbContext _db;

    public UnitOfWork(ApplicationDbContext db, 
        UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager,
        ITokenHandler jwt, IMapper mapper)
    {
        _db = db;

        //Initialize App Repositories
        Genre = new BaseRepository<Genre>(_db.Set<Genre>());
        Movie = new BaseRepository<Movie>(_db.Set<Movie>());
        Auth = new AuthRepository(userManager, roleManager, jwt, mapper);
    }

    public IBaseRepository<Genre> Genre { get; private set; }
    public IBaseRepository<Movie> Movie { get; private set; }
    public IAuthRepository Auth { get; private set; }

    public void Save() => _db.SaveChanges();
}
