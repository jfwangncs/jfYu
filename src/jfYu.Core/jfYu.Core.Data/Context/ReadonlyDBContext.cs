using Microsoft.EntityFrameworkCore;
namespace jfYu.Core.Data.Context
{
    /// <summary>
    /// Read-only  DBContext
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public class ReadonlyDBContext<T>(T current) where T : DbContext
    {
        /// <summary>
        ///Read-only  DBContext
        /// </summary>
        public T Current { get; } = current;

    }
}