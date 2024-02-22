using jfYu.Core.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace jfYu.Core.Data
{
    public class Service<T, TContext>(TContext context, ReadonlyDBContext<TContext> readonlyContext) : IService<T, TContext> where T : BaseEntity
        where TContext : DbContext, IJfYuDbContextService
    {
        public TContext Context { get; } = context;
        public TContext ReadonlyContext { get; } = readonlyContext?.Current ?? context;

        public virtual async Task<int> AddAsync(T entity)
        {
            entity.CreatedTime = entity.UpdatedTime = DateTime.Now;
            await Context.AddAsync(entity);
            return await Context.SaveChangesAsync();
        }



        public virtual async Task<int> AddRangeAsync(List<T> list)
        {
            list.ForEach(entity => { entity.CreatedTime = entity.UpdatedTime = DateTime.Now; });
            await Context.AddRangeAsync(list);
            return await Context.SaveChangesAsync();
        }


        public virtual async Task<int> UpdateAsync(T entity)
        {
            entity.UpdatedTime = DateTime.Now;
            Context.Update(entity);
            return await Context.SaveChangesAsync();
        }
        public virtual async Task<int> UpdateRangeAsync(List<T> list)
        {
            list.ForEach(entity =>
            {
                entity.UpdatedTime = DateTime.Now;
                Context.Update(entity);
            });
            return await Context.SaveChangesAsync();
        }


        public virtual async Task<int> UpdateAsync(Expression<Func<T, bool>>? predicate = null, Action<T>? scalar = null)
        {
            if (predicate == null || scalar == null)
                return 0;
            var data = Context.Set<T>().Where(predicate).ToList();
            data.ForEach(scalar);
            return await Context.SaveChangesAsync();
        }

        public virtual async Task<int> RemoveAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
                return 0;
            var list = GetList(predicate);
            foreach (var entity in list)
            {
                entity.UpdatedTime = DateTime.Now;
                entity.State = (int)StateEnum.Disable;
                Context.Update(entity);
            }
            return await Context.SaveChangesAsync();
        }

        public virtual async Task<int> HardRemoveAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
                return 0;

            var lists = GetList(predicate);
            foreach (var entity in lists)
            {
                Context.Remove(entity);
            }
            return await Context.SaveChangesAsync();

        }
        public virtual async Task<T?> GetOneAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate switch
            {
                null => default,
                _ => await ReadonlyContext.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate)
            };
        }
        public virtual IQueryable<T> GetList(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate switch
            {
                null => ReadonlyContext.Set<T>().AsNoTracking().AsQueryable(),
                _ => ReadonlyContext.Set<T>().Where(predicate).AsNoTracking().AsQueryable()
            };
        }

        public virtual IQueryable<T1> GetList<T1>(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, T1>>? scalar = null)
        {
            if (scalar == null)
                return new List<T1>().AsQueryable();

            return predicate switch
            {
                null => ReadonlyContext.Set<T>().AsNoTracking().Select(scalar).AsQueryable(),
                _ => ReadonlyContext.Set<T>().Where(predicate).AsNoTracking().Select(scalar).AsQueryable()
            };
        }
    }
}
