using Movies.Domain.Entities;

namespace Movies.Domain.Interfaces;

public interface IUsersRepository : IGenericRepository<ImdbUser>
{
    ImdbUser GetUserWithNameBookmarks(int id);
    ImdbUser GetUserWithTitleBookmarks(int id);
    ImdbUser GetUserWithRatingHistory(int id);
    ImdbUser GetUserWithSearchHistory(int id);
    void DeleteAllSearchHistoryFromUser(int id);
}
