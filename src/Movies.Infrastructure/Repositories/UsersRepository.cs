using Microsoft.EntityFrameworkCore;
using Movies.Domain.Entities;
using Movies.Domain.Interfaces;
using Movies.Infrastructure.DataContext;

namespace Movies.Infrastructure.Repositories;

public class UsersRepository : GenericRepository<ImdbUser>, IUsersRepository

{
    public UsersRepository(PostgresDbContext context) : base(context){}

    public ImdbUser GetUserWithNameBookmarks(int id)
    {
        return _context
                .Set<ImdbUser>()
                .Include(x => x.UserBookmarkNames)
                .FirstOrDefault(x => x.Userid == id);
    }

    public ImdbUser GetUserWithTitleBookmarks(int id)
    {
        return _context
                .Set<ImdbUser>()
                .Include(x => x.UserBookmarkTitles)
                .FirstOrDefault(x => x.Userid == id);
    }

    public ImdbUser GetUserWithRatingHistory(int id)
    {
        return _context
                .Set<ImdbUser>()
                .Include(x => x.RatingHistories)
                .FirstOrDefault(x => x.Userid == id);
    }

    public ImdbUser GetUserWithSearchHistory(int id)
    {
        return _context
                .Set<ImdbUser>()
                .Include(x => x.SearchHistories)
                .FirstOrDefault(x => x.Userid == id);
    }

    public void DeleteAllSearchHistoryFromUser(int id)
    {
        var user = _context
                    .Set<ImdbUser>()
                    .Include(x => x.SearchHistories)
                    .FirstOrDefault(x => x.Userid == id);
        _context
            .RemoveRange(user.SearchHistories);
    }
}

