using Movies.DataAccess.Repositories;
using Movies.Core.Models;
using Movies.Core.Interfaces;
using Movies.Core.Interfaces.Repositories;

namespace Movies.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        //The only place can access database 
        private readonly ApplicationDbContext _db;
       
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;

            //Initialize App Repositories
            Genre = new BaseRepository<Genre>(_db.Set<Genre>());
            
        }

        public IBaseRepository<Genre> Genre { get; private set; }

        public void Save() => _db.SaveChanges();
    }
}