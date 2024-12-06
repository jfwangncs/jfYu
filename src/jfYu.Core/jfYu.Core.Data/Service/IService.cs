using jfYu.Core.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace jfYu.Core.Data.Service
{
    public interface IService<T, TContext> where T : BaseEntity
         where TContext : DbContext, IJfYuDbContextService
    {
        /// <summary>
        /// master context
        /// </summary>
        public TContext Context { get; }

        /// <summary>
        /// readonly context
        /// </summary>
        public TContext ReadonlyContext { get; }

        /// <summary>
        /// add
        /// </summary>
        /// <param name="entity">data</param>
        /// <returns>successful/failed</returns>
        Task<int> AddAsync(T entity);

        /// <summary>
        /// add
        /// </summary>
        /// <param name="list">data</param>
        /// <returns>successful/failed</returns>
        Task<int> AddRangeAsync(List<T> list);

        /// <summary>
        /// update
        /// </summary>
        /// <param name="entity">data</param>
        /// <returns>successful/failed</returns>
        Task<int> UpdateAsync(T entity);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="list">data</param>
        /// <returns>successful/failed</returns>
        Task<int> UpdateRangeAsync(List<T> list);

        /// <summary>
        /// update
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <param name="scalar">scalar</param>
        /// <returns>successful/failed</returns>
        Task<int> UpdateAsync(Expression<Func<T, bool>>? predicate = null, Action<T>? scalar = null);


        /// <summary>
        /// remove
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns>successful/failed</returns>
        Task<int> RemoveAsync(Expression<Func<T, bool>>? predicate = null);

        /// <summary>
        /// hard remove
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns>successful/failed</returns>
        Task<int> HardRemoveAsync(Expression<Func<T, bool>>? predicate = null);


        /// <summary>
        /// get one
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns>data</returns>
        Task<T?> GetOneAsync(Expression<Func<T, bool>>? predicate = null);

        /// <summary>
        /// get list
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns>list</returns>
        IQueryable<T> GetList(Expression<Func<T, bool>>? predicate = null);

        /// <summary>
        /// getlist
        /// </summary>
        /// <typeparam name="T1">model</typeparam>
        /// <param name="predicate">predicate</param>
        /// <param name="scalar">Partial column，example:q=>new {id=q.id,name=q.name}、q=>new ClassA{id=q.id,name=q.name}</param>
        /// <returns>list</returns>
        IQueryable<T1> GetList<T1>(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, T1>>? scalar = null);
 
    }
}
