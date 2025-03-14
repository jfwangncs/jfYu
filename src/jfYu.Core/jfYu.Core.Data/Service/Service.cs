using jfYu.Core.Data.Context;
using jfYu.Core.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace jfYu.Core.Data.Service
{
    public class Service<T, TContext>(TContext context, ReadonlyDBContext<TContext> readonlyDBContext) : IService<T, TContext> where T : BaseEntity
        where TContext : DbContext
    {
        public TContext Context { get; } = context;
        public TContext ReadonlyContext { get; } = readonlyDBContext.Current;

        public virtual async Task<int> AddAsync(T entity)
        {
            entity.CreatedTime = entity.UpdatedTime = DateTime.UtcNow;
            await Context.AddAsync(entity);
            return await Context.SaveChangesAsync();
        }

        public virtual async Task<int> AddAsync(List<T> list)
        {
            list.ForEach(entity => { entity.CreatedTime = entity.UpdatedTime = DateTime.UtcNow; });
            await Context.AddRangeAsync(list);
            return await Context.SaveChangesAsync();
        }

        public virtual async Task<int> UpdateAsync(T entity)
        {
            entity.UpdatedTime = DateTime.UtcNow;
            Context.Update(entity);
            return await Context.SaveChangesAsync();
        }

        public virtual async Task<int> UpdateAsync(List<T> list)
        {
            list.ForEach(entity =>
            {
                entity.UpdatedTime = DateTime.UtcNow;
                Context.Update(entity);
            });
            return await Context.SaveChangesAsync();

        }

        public virtual async Task<int> UpdateAsync(Expression<Func<T, bool>> predicate, Action<int, T> selector)
        {
            if (predicate == null || selector == null)
                return 0;
            var data = Context.Set<T>().Where(predicate).ToList();
            for (int i = 0; i < data.Count; i++)
            {
                selector(i, data[i]);
                data[i].UpdatedTime = DateTime.UtcNow;
            }

            return await Context.SaveChangesAsync();
        }
        public virtual async Task<int> RemoveAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                return 0;
            var list = await GetListAsync(predicate);
            foreach (var entity in list)
            {
                entity.UpdatedTime = DateTime.UtcNow;
                entity.State = (int)StateEnum.Disable;
                Context.Update(entity);
            }
            return await Context.SaveChangesAsync();
        }

        public virtual async Task<int> HardRemoveAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                return 0;

            var lists = await GetListAsync(predicate);
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
                null => await ReadonlyContext.Set<T>().FirstOrDefaultAsync(),
                _ => await ReadonlyContext.Set<T>().FirstOrDefaultAsync(predicate)
            };
        }

        public virtual async Task<IList<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate switch
            {
                null => await ReadonlyContext.Set<T>().ToListAsync(),
                _ => await ReadonlyContext.Set<T>().Where(predicate).ToListAsync()
            };
        }

        public virtual async Task<IList<T1>> GetListAsync<T1>(Func<T, T1> selector, Expression<Func<T, bool>>? predicate = null)
        {
            if (selector == null)
                return [];
             
            return predicate switch
            {
                null => (await ReadonlyContext.Set<T>().ToListAsync()).Select(selector).ToList(),
                _ => (await ReadonlyContext.Set<T>().Where(predicate).ToListAsync()).Select(selector).ToList()
            };
        }

        public virtual async Task<IList<T1>> GetSelectListAsync<T1>(Expression<Func<T, T1>> selector, Expression<Func<T, bool>>? predicate = null)
        {
            if (selector == null)
                return [];
            return predicate switch
            {
                null => await ReadonlyContext.Set<T>().Select(selector).ToListAsync(),
                _ => await ReadonlyContext.Set<T>().Where(predicate).Select(selector).ToListAsync()
            };
        }
    }
}
