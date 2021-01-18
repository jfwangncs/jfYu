using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace jfYu.Core.Data
{
    public interface IService<T> where T : BaseEntity
    {
        #region 新增
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">数据</param>
        /// <returns>是否成功</returns>
        bool Add(T entity);
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">数据</param>
        /// <returns>是否成功</returns>
        Task<bool> AddAsync(T entity);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="list">列表</param>
        /// <returns>是否成功</returns>
        bool AddRange(List<T> list);
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="list">列表</param>
        /// <returns>是否成功</returns>
        Task<bool> AddRangeAsync(List<T> list);
        #endregion

        #region 更新

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>是否成功</returns>
        bool Update(T entity);

        bool Update(Expression<Func<T, bool>> predicate = null, Action<T> scalar = null);

        Task<bool> UpdateAsync(Expression<Func<T, bool>> predicate = null, Action<T> scalar = null);
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateAsync(T entity);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="list">实体列表</param>
        /// <returns>是否成功</returns>
        bool UpdateRange(List<T> list);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="list">实体列表</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateRangeAsync(List<T> list);

        #endregion

        #region 删除
        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="guid">guid</param>
        /// <returns>是否成功</returns>
        bool Remove(Guid guid);

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="guid">guid</param>
        /// <returns>是否成功</returns>
        bool Remove(string guid);

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="guid">guid</param>
        /// <returns>是否成功</returns>
        Task<bool> RemoveAsync(Guid guid);

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="guid">guid</param>
        /// <returns>是否成功</returns>
        Task<bool> RemoveAsync(string guid);

        /// <summary>
        /// 硬删除
        /// </summary>
        /// <param name="guid">guid</param>
        /// <returns>是否成功</returns>
        bool HardRemove(Guid guid);

        /// <summary>
        /// 硬删除
        /// </summary>
        /// <param name="guid">guid</param>
        /// <returns>是否成功</returns>
        bool HardRemove(string guid);

        /// <summary>
        /// 硬删除
        /// </summary>
        /// <param name="guid">guid</param>
        /// <returns>是否成功</returns>
        Task<bool> HardRemoveAsync(Guid guid);

        /// <summary>
        /// 硬删除
        /// </summary>
        /// <param name="guid">guid</param>
        /// <returns>是否成功</returns>
        Task<bool> HardRemoveAsync(string guid);
        #endregion

        #region 获取单个
        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="guid">guid</param>
        /// <returns>数据</returns>
        T GetById(Guid guid);

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="guid">guid</param>
        /// <returns>数据</returns>
        T GetById(string guid);

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="guid">guid</param>
        /// <returns>数据</returns>
        Task<T> GetByIdAsync(Guid guid);


        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="guid">guid</param>
        /// <returns>数据</returns>
        Task<T> GetByIdAsync(string guid);

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="predicate">筛选条件</param>
        /// <returns>数据</returns>
        T GetSingle(Expression<Func<T, bool>> predicate = null);

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="predicate">筛选条件</param>
        /// <returns>数据</returns>
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate = null);

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="predicate">筛选条件</param>
        /// <returns>数据</returns>
        T GetFirst(Expression<Func<T, bool>> predicate = null);

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="predicate">筛选条件</param>
        /// <returns>数据</returns>
        Task<T> GetFirstAsync(Expression<Func<T, bool>> predicate = null);
        #endregion

        #region 获取列表
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <param name="predicate">筛选条件</param>
        /// <returns>数据集</returns>
        IQueryable<T> GetList(Expression<Func<T, bool>> predicate = null);

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <param name="predicate">筛选条件</param>
        /// <returns>数据集</returns>
        Task<IQueryable<T>> GetListAsync(Expression<Func<T, bool>> predicate = null);


        /// <summary>
        /// 获取部分字段的所有数据
        /// </summary>
        /// <typeparam name="T1">返回数据集类型，可以是动态类型，类</typeparam>
        /// <param name="predicate">筛选条件</param>
        /// <param name="scalar">部分字段，例如:q=>new {id=q.id,name=q.name}、q=>new ClassA{id=q.id,name=q.name}</param>
        /// <returns>数据集</returns>
        IQueryable<T1> GetList<T1>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, T1>> scalar = null);

        /// <summary>
        /// 获取部分字段的所有数据
        /// </summary>
        /// <typeparam name="T1">返回数据集类型，可以是动态类型，类</typeparam>
        /// <param name="predicate">筛选条件</param>
        /// <param name="scalar">部分字段，例如:q=>new {id=q.id,name=q.name}、q=>new ClassA{id=q.id,name=q.name}</param>
        /// <returns>数据集</returns>
        Task<IQueryable<T1>> GetListAsync<T1>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, T1>> scalar = null);
        #endregion

        #region 是否有数据
        /// <summary>
        /// 该id是否存在
        /// </summary>
        /// <param name="guid">guid</param>
        /// <returns>是否存在</returns>
        bool IsExist(Guid guid);

        /// <summary>
        /// 该id是否存在
        /// </summary>
        /// <param name="guid">guid</param>
        /// <returns>是否存在</returns>
        bool IsExist(string guid);


        /// <summary>
        /// 该id是否存在
        /// </summary>
        /// <param name="guid">guid</param>
        /// <returns>是否存在</returns>
        Task<bool> IsExistAsync(Guid guid);

        /// <summary>
        /// 该id是否存在
        /// </summary>
        /// <param name="guid">guid</param>
        /// <returns>是否存在</returns>
        Task<bool> IsExistAsync(string guid);
        #endregion
    }
}
