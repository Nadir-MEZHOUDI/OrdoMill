using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OrdoMill.Helpers.Bases;

namespace OrdoMill.Helpers;

public static class DbSetExtensions
{
    public static TEntity AddOrUpdate<TEntity>(this DbSet<TEntity> dbSet, TEntity entity)
        where TEntity : class, IEntity
    {
        if (entity == null)
        {
            return null;
        }

        var context = dbSet.GetService<ICurrentDbContext>().Context;
        var existingEntity = entity.Id > 0 ? dbSet.Find(entity.Id) : null;

        if (existingEntity == null)
        {
            dbSet.Add(entity);
            return entity;
        }

        context.Entry(existingEntity).CurrentValues.SetValues(entity);
        return existingEntity;
    }

    public static TEntity AddOrUpdate<TEntity, TProperty>(this DbSet<TEntity> dbSet,
        Expression<Func<TEntity, TProperty>> identifierExpression, TEntity entity)
        where TEntity : class, IEntity
    {
        return dbSet.AddOrUpdate(entity);
    }
}
