using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Naming
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ChinookContext>
    {
        public ChinookContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ChinookContext>();
            optionsBuilder
                .UseSqlite("Chinook.db")
                .UseSnakeCaseNamingConvention();

            return new ChinookContext(optionsBuilder.Options);
        }
    }
}
