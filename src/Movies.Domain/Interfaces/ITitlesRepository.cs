using Movies.Domain.Entities;
using Movies.Domain.ValueObjects.SearchResults;

namespace Movies.Domain.Interfaces;

public interface ITitlesRepository : IGenericRepository<Title>
{
    IEnumerable<Title> GetTitlesWithRating(int page, int pageSize);
    Title GetTitleWithRelatedEntities(string id);
    Title GetTitleWithRating(string id);
    bool CreateTitleRating(string titleId, int rate, int userId);
    bool UpdateTitleRating(string titleId, int existingRate, int updatedRate, int userId);
    bool DeleteTitleRating(string titleId, int rate, int userId);
    IQueryable<TitleSearchResults> GetTitleSearchResults(string searchQuery, int userId);
}
