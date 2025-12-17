using Microsoft.EntityFrameworkCore;
using Movies.Domain.Entities;
using Movies.Domain.Interfaces;
using Movies.Domain.ValueObjects.SearchResults;
using Movies.Infrastructure.DataContext;
using Movies.Infrastructure.Functions.ScalarFunctions;

namespace Movies.Infrastructure.Repositories;

public class TitlesRepository : GenericRepository<Title>, ITitlesRepository
{
    public TitlesRepository(PostgresDbContext context) : base(context) { }

    public Title GetTitleWithRelatedEntities(string id)
    {
        return _context.Set<Title>()
            .Include(x => x.TitleGenres)
            .Include(x => x.Localizeds)
            .Include(x => x.Ratings)
            .Include(x => x.Principals)
            .FirstOrDefault(x => x.Tconst == id);
    }

    public Title GetTitleWithRating(string id)
    {
        return _context.Set<Title>()
                .Include(x => x.Ratings)
                .FirstOrDefault(x => x.Tconst == id);
    }

        public bool CreateTitleRating(string titleId, int rate, int userId)
    {
        return _context.Ratings
                    .Select(x => CreateTitleRatingFunction
                    .IsTitleRated(titleId, rate, userId))
                    .FirstOrDefault();
    }

    public bool UpdateTitleRating(string titleId, int existingRate, int updatedRate, int userId)
    {
        return _context.Ratings
                    .Select(x => UpdateTitleRatingFunction
                    .IsTitleRatingUpdated(titleId, existingRate, updatedRate, userId))
                    .FirstOrDefault();
    }

    public bool DeleteTitleRating(string titleId, int rate, int userId)
    {
        return _context.Ratings
                    .Select(x => DeleteTitleRatingFunction
                    .IsTitleRatingDeleted(titleId, rate, userId))
                    .FirstOrDefault();
    }

    public IQueryable<TitleSearchResults> GetTitleSearchResults(string searchQuery, int userId)
    {
        return _context
                .GetTitleSearchResults(searchQuery, userId);
    }
}