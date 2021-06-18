using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EF.Naming
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            // DbContext
            services.AddDbContext<ChinookContext>(options => 
                options.UseSqlite("Data Source=Chinook.db")
                .UseSnakeCaseNamingConvention()
            );
        }
    }
}
