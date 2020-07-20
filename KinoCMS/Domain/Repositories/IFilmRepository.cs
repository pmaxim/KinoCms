using Domain.Entities;
using System.Linq;

namespace Domain.Repositories
{
    public interface IFilmRepository : IBaseRepository<Film>
    {
        IQueryable<Film> Films { get; }
    }
}
