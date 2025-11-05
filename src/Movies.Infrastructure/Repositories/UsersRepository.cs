using Movies.Infrastructure.DataContext;

namespace Movies.Infrastructure.Repositories;

public class UsersRepository<ImdbUser> : GenericRepository<ImdbUser> where ImdbUser : class

{
    public UsersRepository(PostgresDbContext context) : base(context)
    {

    }
    
    public void CreateUser(string UserName, string password)
    {
    
    }
}

