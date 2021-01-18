using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace jfYu.Core.Data
{
    public class Service<T, Q> : IService<T> where T : BaseEntity
        where Q : DbContext
    {

        protected Expression<Func<T, bool>> exprTrue = q => true;

        public Q Master { get; }

        public Q Slave { get; }

        public List<Q> Slaves { get; }

        public Service(IDbContextService<Q> contextService)
        {

            Master = contextService.Master;
            Slave = contextService.Slave;
            Slaves = contextService.Slaves;
        }
        public virtual bool Add(T entity)
        {
            Master.Add(entity);
            return Master.SaveChanges() > 0;
        }

        public virtual async Task<bool> AddAsync(T entity)
        {
            await Master.AddAsync(entity);
            return (await Master.SaveChangesAsync()) > 0;
        }

        public virtual bool AddRange(List<T> list)
        {
            Master.AddRange(list);
            return Master.SaveChanges() > 0;
        }

        public virtual async Task<bool> AddRangeAsync(List<T> list)
        {
            await Master.AddRangeAsync(list);
            return (await Master.SaveChangesAsync()) > 0;
        }

        public virtual bool Update(T entity)
        {
            entity.UpdateTime = DateTime.Now;
            Master.Update(entity);
            return Master.SaveChanges() > 0;
        }
        public virtual async Task<bool> UpdateAsync(T entity)
        {
            entity.UpdateTime = DateTime.Now;
            Master.Update(entity);
            return (await Master.SaveChangesAsync()) > 0;
        }

        public virtual bool Update(Expression<Func<T, bool>> predicate = null, Action<T> scalar = null)
        {
            predicate ??= exprTrue;
            if (scalar == null)
                return false;
            var data = Slave.Set<T>().Where(predicate).ToList();
            data.ForEach(scalar);
            return Master.SaveChanges() > 0;
        }

        public virtual async Task<bool> UpdateAsync(Expression<Func<T, bool>> predicate = null, Action<T> scalar = null)
        {
            predicate ??= exprTrue;
            if (scalar == null)
                return false;
            var data = Slave.Set<T>().Where(predicate).ToList();
            data.ForEach(scalar);
            return (await Master.SaveChangesAsync()) > 0;
        }
        public virtual bool UpdateRange(List<T> list)
        {
            list.ForEach(q =>
            {
                q.UpdateTime = DateTime.Now;
            });
            Master.UpdateRange(list);
            return Master.SaveChanges() > 0;
        }

        public virtual async Task<bool> UpdateRangeAsync(List<T> list)
        {
            list.ForEach(q =>
            {
                q.UpdateTime = DateTime.Now;
            });
            Master.UpdateRange(list);
            return (await Master.SaveChangesAsync()) > 0;
        }

        public virtual bool Remove(Guid id)
        {
            if (IsExist(id))
            {
                var entity = Master.Find<T>(id);
                entity.UpdateTime = DateTime.Now;
                entity.State = StateEnum.Disable;
                return Master.SaveChanges() > 0;
            }
            return false;
        }
        public virtual bool Remove(string id)
        {
            if (IsExist(id))
            {
                var entity = Master.Find<T>(id);
                entity.UpdateTime = DateTime.Now;
                entity.State = StateEnum.Disable;
                return Master.SaveChanges() > 0;
            }
            return false;
        }
        public virtual async Task<bool> RemoveAsync(Guid id)
        {
            if (await IsExistAsync(id))
            {
                var entity = await Master.FindAsync<T>(id);
                entity.UpdateTime = DateTime.Now;
                entity.State = StateEnum.Disable;
                return (await Master.SaveChangesAsync()) > 0;
            }
            return false;
        }
        public virtual async Task<bool> RemoveAsync(string id)
        {
            if (await IsExistAsync(id))
            {
                var entity = await Master.FindAsync<T>(id);
                entity.UpdateTime = DateTime.Now;
                entity.State = StateEnum.Disable;
                return (await Master.SaveChangesAsync()) > 0;
            }
            return false;
        }

        public virtual bool HardRemove(Guid id)
        {
            if (IsExist(id))
            {
                var entity = Master.Find<T>(id);
                Master.Remove(entity);
                return Master.SaveChanges() > 0;
            }
            return false;
        }
        public virtual bool HardRemove(string id)
        {
            if (IsExist(id))
            {
                var entity = Master.Find<T>(id);
                Master.Remove(entity);
                return Master.SaveChanges() > 0;
            }
            return false;
        }

        public virtual async Task<bool> HardRemoveAsync(Guid id)
        {
            if (await IsExistAsync(id))
            {
                var entity = await Master.FindAsync<T>(id);
                Master.Remove(entity);
                return (await Master.SaveChangesAsync()) > 0;
            }
            return false;
        }
        public virtual async Task<bool> HardRemoveAsync(string id)
        {
            if (await IsExistAsync(id))
            {
                var entity = await Master.FindAsync<T>(id);
                Master.Remove(entity);
                return (await Master.SaveChangesAsync()) > 0;
            }
            return false;
        }
        public virtual bool IsExist(Guid id)
        {
            return Slave.Set<T>().Any(q => q.Id.Equals(id));
        }
        public virtual bool IsExist(string id)
        {
            return Slave.Set<T>().Any(q => q.Id.Equals(id));
        }
        public virtual async Task<bool> IsExistAsync(Guid id)
        {
            return await Slave.Set<T>().AnyAsync(q => q.Id.Equals(id));
        }
        public virtual async Task<bool> IsExistAsync(string id)
        {
            return await Slave.Set<T>().AnyAsync(q => q.Id.Equals(id));
        }
        public virtual T GetById(Guid id)
        {
            return Slave.Find<T>(id);
        }
        public virtual T GetById(string id)
        {
            return Slave.Find<T>(id);
        }
        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await Slave.FindAsync<T>(id);
        }
        public virtual async Task<T> GetByIdAsync(string id)
        {
            return await Slave.FindAsync<T>(id);
        }

        public virtual T GetSingle(Expression<Func<T, bool>> predicate = null)
        {
            predicate ??= exprTrue;
            return Slave.Set<T>().Where(predicate).SingleOrDefault();
        }
        public virtual async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate = null)
        {
            predicate ??= exprTrue;
            return await Slave.Set<T>().Where(predicate).SingleOrDefaultAsync();
        }

        public virtual T GetFirst(Expression<Func<T, bool>> predicate = null)
        {
            predicate ??= exprTrue;
            return Slave.Set<T>().Where(predicate).FirstOrDefault();
        }
        public virtual async Task<T> GetFirstAsync(Expression<Func<T, bool>> predicate = null)
        {
            predicate ??= exprTrue;
            return await Slave.Set<T>().Where(predicate).FirstOrDefaultAsync();
        }

        public virtual IQueryable<T> GetList(Expression<Func<T, bool>> predicate = null)
        {
            predicate ??= exprTrue;
            return Slave.Set<T>().Where(predicate).AsQueryable();
        }
        public virtual async Task<IQueryable<T>> GetListAsync(Expression<Func<T, bool>> predicate = null)
        {
            predicate ??= exprTrue;
            return await Task.Run(() => Slave.Set<T>().Where(predicate).AsQueryable());
        }
        public virtual IQueryable<T1> GetList<T1>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, T1>> scalar = null)
        {
            predicate ??= exprTrue;
            if (scalar == null)
                return new List<T1>().AsQueryable();
            return Slave.Set<T>().Where(predicate).Select(scalar).AsQueryable();
        }

        public virtual async Task<IQueryable<T1>> GetListAsync<T1>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, T1>> scalar = null)
        {
            predicate ??= exprTrue;
            if (scalar == null)
                return new List<T1>().AsQueryable();
            return await Task.Run(() => Slave.Set<T>().Where(predicate).Select(scalar).AsQueryable());
        }

    }
}
