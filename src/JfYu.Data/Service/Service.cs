﻿using JfYu.Data.Context;
using JfYu.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JfYu.Data.Service
{
    /// <summary>
    /// Services
    /// </summary>
    /// <typeparam name="T">Entity Model</typeparam>
    /// <typeparam name="TContext">Db Context</typeparam>
    public class Service<T, TContext>(TContext context, ReadonlyDBContext<TContext> readonlyDBContext) : IService<T, TContext> where T : BaseEntity
            where TContext : DbContext
    {
        /// <inheritdoc/>
        public TContext Context { get; } = context;

        /// <inheritdoc/>
        public TContext ReadonlyContext { get; } = readonlyDBContext.Current;

        /// <inheritdoc/>
        public virtual async Task<int> AddAsync(T entity)
        {
            entity.CreatedTime = entity.UpdatedTime = DateTime.UtcNow;
            await Context.AddAsync(entity).ConfigureAwait(false);
            return await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<int> AddAsync(List<T> list)
        {
            list.ForEach(entity => { entity.CreatedTime = entity.UpdatedTime = DateTime.UtcNow; });
            await Context.AddRangeAsync(list).ConfigureAwait(false);
            return await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<int> UpdateAsync(T entity)
        {
            entity.UpdatedTime = DateTime.UtcNow;
            Context.Update(entity);
            int saveChanges = await Context.SaveChangesAsync().ConfigureAwait(false);
            return saveChanges;
        }

        /// <inheritdoc/>
        public virtual async Task<int> UpdateAsync(List<T> list)
        {
            list.ForEach(entity =>
            {
                entity.UpdatedTime = DateTime.UtcNow;
            });
            Context.UpdateRange(list);
            int saveChanges = await Context.SaveChangesAsync().ConfigureAwait(false);
            return saveChanges;
        }

        /// <inheritdoc/>
        public virtual async Task<int> UpdateAsync(Expression<Func<T, bool>> predicate, Action<int, T> selector)
        {
            if (predicate == null || selector == null)
                return 0;
            var data = await Context.Set<T>().Where(predicate).ToListAsync().ConfigureAwait(false);
            for (int i = 0; i < data.Count; i++)
            {
                selector(i, data[i]);
                data[i].UpdatedTime = DateTime.UtcNow;
            }

            return await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<int> RemoveAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                return 0;
            var list = await GetListAsync(predicate).ConfigureAwait(false);
            foreach (var entity in list)
            {
                entity.UpdatedTime = DateTime.UtcNow;
                entity.Status = (int)DataStatus.Disable;
                Context.Update(entity);
            }
            return await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<int> HardRemoveAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                return 0;

            var lists = await GetListAsync(predicate).ConfigureAwait(false);
            foreach (var entity in lists)
            {
                Context.Remove(entity);
            }
            return await Context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async Task<T?> GetOneAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate switch
            {
                null => await ReadonlyContext.Set<T>().FirstOrDefaultAsync().ConfigureAwait(false),
                _ => await ReadonlyContext.Set<T>().FirstOrDefaultAsync(predicate).ConfigureAwait(false)
            };
        }

        /// <inheritdoc/>
        public virtual async Task<IList<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate switch
            {
                null => await ReadonlyContext.Set<T>().ToListAsync().ConfigureAwait(false),
                _ => await ReadonlyContext.Set<T>().Where(predicate).ToListAsync().ConfigureAwait(false)
            };
        }

        /// <inheritdoc/>
        public virtual async Task<IList<T1>> GetSelectListAsync<T1>(Expression<Func<T, T1>> selector, Expression<Func<T, bool>>? predicate = null)
        {
            if (selector == null)
                return [];
            return predicate switch
            {
                null => await ReadonlyContext.Set<T>().Select(selector).ToListAsync().ConfigureAwait(false),
                _ => await ReadonlyContext.Set<T>().Where(predicate).Select(selector).ToListAsync().ConfigureAwait(false)
            };
        }
    }
}