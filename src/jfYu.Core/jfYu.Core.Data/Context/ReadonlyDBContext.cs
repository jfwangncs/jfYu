using Microsoft.EntityFrameworkCore;
namespace jfYu.Core.Data.Context
{
    /// <summary>
    /// readonly  DBContext
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public class ReadonlyDBContext<T>(T current) where T : DbContext
    {
        public T Current { get; } = current;

    }
}