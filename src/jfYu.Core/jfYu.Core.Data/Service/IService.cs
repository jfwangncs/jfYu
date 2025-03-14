using jfYu.Core.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace jfYu.Core.Data.Service
{
    public interface IService<T, TContext> where T : BaseEntity
         where TContext : DbContext
    {
        /// <summary>
        /// Master context
        /// </summary>
        public TContext Context { get; }

        /// <summary>
        /// Readonly context
        /// </summary>
        public TContext ReadonlyContext { get; }

        /// <summary>
        /// Adds data
        /// </summary>
        /// <param name="entity">data</param>
        /// <returns>Number of records</returns>
        Task<int> AddAsync(T entity);

        /// <summary>
        /// Adds list 
        /// </summary>
        /// <param name="list">data</param>
        /// <returns>Number of records</returns>
        Task<int> AddAsync(List<T> list);

        /// <summary>
        /// Updates
        /// </summary>
        /// <param name="entity">data</param>
        /// <returns>Number of records</returns>
        Task<int> UpdateAsync(T entity);

        /// <summary>
        /// Updates list
        /// </summary>
        /// <param name="list">data</param>
        /// <returns>Number of records</returns>
        Task<int> UpdateAsync(List<T> list);

        /// <summary>
        /// Updates records matching the given predicate, applying a custom action on each 
        /// </summary>
        /// <param name="predicate">Condition to filter records.</param>
        /// <param name="selector">Action to apply on each record, with its index and entity.</param>
        /// <returns>Number of records</returns>
        Task<int> UpdateAsync(Expression<Func<T, bool>> predicate, Action<int, T> selector);


        /// <summary>
        /// Soft Removes records matching the given predicate
        /// </summary>
        /// <param name="predicate">Condition to filter records.</param>
        /// <returns>Number of records</returns>
        Task<int> RemoveAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Hard Removes records matching the given predicate
        /// </summary>
        /// <param name="predicate">Condition to filter records.</param>
        /// <returns>Number of records</returns>
        Task<int> HardRemoveAsync(Expression<Func<T, bool>> predicate);


        /// <summary>
        /// Get one record matching the given predicate
        /// </summary>
        /// <param name="predicate">Condition to filter records.</param>
        /// <returns>Data of record</returns>
        Task<T?> GetOneAsync(Expression<Func<T, bool>>? predicate = null);

        /// <summary>
        /// Get list matching the given predicate
        /// </summary>
        /// <param name="predicate">Condition to filter records.</param>
        /// <returns>Data of records</returns>
        Task<IList<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null);

        /// <summary>
        /// Get list matching the given predicate, with a projection at Memory
        /// </summary>
        /// <typeparam name="T1">The type of the result after projection.</typeparam>
        /// <param name="selector">A function to project the entity to a different type，example:q=>new {id=q.id,name=q.name},q=>new ClassA{id=q.id,name=q.name}</param>
        /// <param name="predicate">Condition to filter records.</param>
        /// <returns>Data of records</returns>
        Task<IList<T1>> GetListAsync<T1>(Func<T, T1> selector, Expression<Func<T, bool>>? predicate = null);


        /// <summary>
        /// Get list matching the given predicate, with a projection at Database
        /// </summary>
        /// <typeparam name="T1">The type of the result after projection.</typeparam>
        /// <param name="selector">A function to project the entity to a different type，example:q=>new {id=q.id,name=q.name},q=>new ClassA{id=q.id,name=q.name}</param>
        /// <param name="predicate">Condition to filter records.</param>
        /// <returns>Data of records</returns>
        Task<IList<T1>> GetSelectListAsync<T1>(Expression<Func<T, T1>> selector, Expression<Func<T, bool>>? predicate = null);
    }
}
