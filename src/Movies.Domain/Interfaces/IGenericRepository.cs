using System.Linq.Expressions;

namespace Movies.Domain.Interfaces;

public interface IGenericRepository<TEntity> where TEntity : class
{
    void CreateEntity(TEntity entity);
    TEntity? FindEntity(object id);
    TEntity? RetrieveEntity(Expression<Func<TEntity, bool>> predicate);
    IEnumerable<TEntity> RetrieveEntities(int page, int pageSize);
    void UpdateEntity(TEntity entity);
    void DeleteEntity(TEntity entity);
    void DeleteEntities(IEnumerable<TEntity> entities);
    IEnumerable<TEntity> FindEntities(Expression<Func<TEntity, bool>> predicate, int page, int pageSize);
    int GetEntityCount(Expression<Func<TEntity, bool>> predicate);
}
