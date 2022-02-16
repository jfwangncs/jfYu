using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace jfYu.Core.Data
{
    public interface IDbContextService<T> where T : DbContext
    {
        T Master { get; }
        T Slave { get; }
        List<T> Slaves { get; }
    }
}