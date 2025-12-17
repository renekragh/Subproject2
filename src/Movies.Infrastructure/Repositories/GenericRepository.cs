using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Movies.Domain.Interfaces;
using Movies.Infrastructure.DataContext;

namespace Movies.Infrastructure.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    protected readonly PostgresDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;
  
    public GenericRepository(PostgresDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public void CreateEntity(TEntity entity)
    {
        _dbSet.Add(entity);
    }

    public void DeleteEntity(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public void DeleteEntities(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public IEnumerable<TEntity> RetrieveEntities(int page, int pageSize)
    {
        return _dbSet
                .Skip(page * pageSize)
                .Take(pageSize);
                //.ToList();
    }


    public TEntity FindEntity(object id)
    {
        return _dbSet.Find(id);
    }

    public TEntity RetrieveEntity(Expression<Func<TEntity, bool>> predicate)
    {
        return _dbSet
                .Where(predicate).FirstOrDefault();
    }

    public void UpdateEntity(TEntity entity)
    {
        _dbSet.Update(entity);
       // _dbSet.Entry(entity).State = EntityState.Modified;

    }

    public IEnumerable<TEntity> FindEntities(Expression<Func<TEntity, bool>> predicate, int page, int pageSize)
    {
        return _dbSet
        .Where(predicate)
        .Skip(page * pageSize)
        .Take(pageSize);
       // .ToList();
    }

    public int GetEntityCount(Expression<Func<TEntity, bool>> predicate)
    {
        if (predicate is null) return _dbSet.Count();
        return _dbSet
        .Where(predicate)
        .Count();
    }
}
