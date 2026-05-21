using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DnDreams.Infrastructure.Persistence;

namespace DnDreams.Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DnDreamsDbContext>
    {
        public DnDreamsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DnDreamsDbContext>();

            optionsBuilder.UseSqlite("Data Source=migration_temp.db3");

            return new DnDreamsDbContext(optionsBuilder.Options);
        }
    }
}