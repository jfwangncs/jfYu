using jfYu.Core.Test.Models;
using Microsoft.EntityFrameworkCore;

namespace jfYu.Core.Test.MemoryDB
{
    public class TestDbContext : DbContext
    {
        public DbSet<TestModel> TestModels { get; set; }
        public DbSet<TestSubModel> TestSubModels { get; set; }

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource=localdatabase.db");
        }
    }
}