using jfYu.Core.Test.Models;
#if NET8_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#else
using System.Data.Entity;
#endif
namespace jfYu.Core.Test.MemoryDB
{
#if NET8_0_OR_GREATER
    public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
    {
        public DbSet<TestModel> TestModels { get; set; }
        public DbSet<TestSubModel> TestSubModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource=localdatabase.db");
        }
    }
#else
    public class TestDbContext(string connStr) : DbContext(connStr)
    {
        public DbSet<TestModel> TestModels { get; set; }= null!;
        public DbSet<TestSubModel> TestSubModels { get; set; }  = null!;           
    }
#endif
}
