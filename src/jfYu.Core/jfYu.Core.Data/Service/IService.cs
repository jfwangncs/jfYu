using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace jfYu.Core.Data
{
    public interface IService<T> where T : BaseEntity
    {
        /// <summary>
        /// 新增/修改
        /// </summary>
        /// <param name="entity">数据</param>
        /// <returns>是否成功</returns>
        bool AddOrUpdate(T entity);

        /// <summary>
        /// 新增/修改
        /// </summary>
        /// <param name="entity">数据</param>
        /// <returns>是否成功</returns>
        Task<bool> AddOrUpdateAsync(T entity);

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
        /// <param name="id">id</param>
        /// <returns>是否成功</returns>
        bool Remove(Guid id);

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>是否成功</returns>
        bool Remove(string id);

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns>是否成功</returns>
        bool Remove(Expression<Func<T, bool>> predicate = null);

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>是否成功</returns>
        Task<bool> RemoveAsync(Guid id);

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>是否成功</returns>
        Task<bool> RemoveAsync(string id);
        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns>是否成功</returns>
        Task<bool> RemoveAsync(Expression<Func<T, bool>> predicate = null);

        /// <summary>
        /// 硬删除
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>是否成功</returns>
        bool HardRemove(Guid id);

        /// <summary>
        /// 硬删除
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>是否成功</returns>
        bool HardRemove(string id);

        /// <summary>
        /// 硬删除
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>是否成功</returns>
        Task<bool> HardRemoveAsync(Guid id);

        /// <summary>
        /// 硬删除
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>是否成功</returns>
        Task<bool> HardRemoveAsync(string id);
        #endregion

        #region 根据id获取实体
        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>数据</returns>
        T GetById(Guid id);

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>数据</returns>
        T GetById(string id);

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>数据</returns>
        Task<T> GetByIdAsync(Guid id);


        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>数据</returns>
        Task<T> GetByIdAsync(string id);

        #endregion

        #region 获取单个实体
        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns>数据</returns>
        T GetOne(Expression<Func<T, bool>> predicate = null);

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns>数据</returns>
        Task<T> GetOneAsync(Expression<Func<T, bool>> predicate = null);
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
        /// <param name="id">id</param>
        /// <returns>是否存在</returns>
        bool IsExist(Guid id);

        /// <summary>
        /// 该id是否存在
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>是否存在</returns>
        bool IsExist(string id);

        /// <summary>
        /// 该id是否存在
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns>是否存在</returns>
        bool IsExist(Expression<Func<T, bool>> predicate = null);

        /// <summary>
        /// 该id是否存在
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>是否存在</returns>
        Task<bool> IsExistAsync(Guid id);

        /// <summary>
        /// 该id是否存在
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>是否存在</returns>
        Task<bool> IsExistAsync(string id);

        /// <summary>
        /// 该id是否存在
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns>是否存在</returns>
        Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate = null);


        #endregion
    }
}
