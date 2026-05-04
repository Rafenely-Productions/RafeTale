using DnDreams.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreamsInfrastructure.Persistance
{
    public class DnDreamsDbContextFactory : IDesignTimeDbContextFactory<DnDreamsDbContext>
    {
        public DnDreamsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DnDreamsDbContext>();

            // Aquí usamos una ruta temporal para la migración
            optionsBuilder.UseSqlite("Data Source=design_time_temp.db");

            return new DnDreamsDbContext(optionsBuilder.Options);
        }
    }
}
