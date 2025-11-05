
using Movies.Domain.Interfaces;

namespace Movies.Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    bool Save();
}
