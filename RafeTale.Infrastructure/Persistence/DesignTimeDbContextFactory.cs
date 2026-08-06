using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using RafeTale.Infrastructure.Persistence;

namespace RafeTale.Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<RafeTaleDbContext>
    {
        public RafeTaleDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RafeTaleDbContext>();

            optionsBuilder.UseSqlite("Data Source=migration_temp.db3");

            return new RafeTaleDbContext(optionsBuilder.Options);
        }
    }
}