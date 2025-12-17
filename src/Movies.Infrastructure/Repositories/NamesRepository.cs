using Microsoft.EntityFrameworkCore;
using Movies.Domain.Entities;
using Movies.Domain.Interfaces;
using Movies.Infrastructure.DataContext;

namespace Movies.Infrastructure.Repositories;

public class NamesRepository : GenericRepository<Name>, INamesRepository
{
    public NamesRepository(PostgresDbContext context) : base(context) { }

    public Name GetNameWithRelatedEntities(string id)
    {
        return _context.Set<Name>()
            .Include(x => x.NamePrimaryProfessions)
            .Include(x => x.NameKnownForTitles)
            .Include(x => x.Principals)
            .FirstOrDefault(x => x.Nconst == id);
    }
}
