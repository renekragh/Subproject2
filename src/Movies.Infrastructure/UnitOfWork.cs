using Movies.Application.Common.Interfaces;
using Movies.Domain.Interfaces;
using Movies.Infrastructure.DataContext;
using Movies.Infrastructure.Repositories;

namespace Movies.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private PostgresDbContext _dbContext;
    private readonly Dictionary<Type, object> _repositories;
    private bool disposed = false;
    public INamesRepository NamesRepository { get; }
    public ITitlesRepository TitlesRepository { get; }
    public IUsersRepository UsersRepository { get; }


    public UnitOfWork(PostgresDbContext dbContext, INamesRepository namesRepository, ITitlesRepository titlesRepository, IUsersRepository usersRepository)
    {
        _dbContext = dbContext;
        _repositories = new Dictionary<Type, object>();
        NamesRepository = namesRepository;
        TitlesRepository = titlesRepository;
        UsersRepository = usersRepository;
    }

    public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        var existingRepository = _repositories.GetValueOrDefault(typeof(TEntity));
        if (existingRepository != null) return (IGenericRepository<TEntity>) existingRepository;
        var newRepository = new GenericRepository<TEntity>(_dbContext);
        _repositories.Add(typeof(TEntity), newRepository);
        return newRepository;
    }

    public bool Save()
    {
        return _dbContext.SaveChanges() > 0;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }
        disposed = true;
    }
}
