using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace jfYu.Core.Data
{


    public abstract class BaseReadonlyDBContext : DbContext
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <returns></returns>

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(0);
        }
    }

    /// <summary>
    /// readonly  DBContext
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public class ReadonlyDBContext<T>(IContextRead current) : BaseReadonlyDBContext where T : DbContext, IJfYuDbContextService
    {
        public T Current { get; } = (T)current;

    }
}