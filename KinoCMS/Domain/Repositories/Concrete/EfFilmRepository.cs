using Domain.Entities;
using System.Linq;

namespace Domain.Repositories.Concrete
{
    public class EfFilmRepository : EfBaseRepository<Film>, IFilmRepository
    {
        public IQueryable<Film> Films => Context.Films;

        public EfFilmRepository(EfDbContext db) : base(db)
        {
        }
    }
}
