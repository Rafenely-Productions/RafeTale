using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Rafedream.Infrastructure.Persistence;

namespace Rafedream.Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<RafedreamDbContext>
    {
        public RafedreamDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RafedreamDbContext>();

            optionsBuilder.UseSqlite("Data Source=migration_temp.db3");

            return new RafedreamDbContext(optionsBuilder.Options);
        }
    }
}