using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepareDb
    {
        private static void SeedData(ApplicationDbContext context, bool isProduction)
        {
            if(isProduction)
            {
                Console.WriteLine("Attmepting to apply migrations...");

                try
                {
                    context.Database.Migrate();
                }

                catch(Exception ex)
                {
                    Console.WriteLine($"--> Could not run migrations: {ex.Message}");
                }
            }

            if(!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding Data...");

                context.Platforms.AddRange(
                    new Platform { Name = "Dot Net", Publisher = "Microsoft", Cost = "free" },
                    new Platform { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "free" },
                    new Platform { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "free" });

                context.SaveChanges();
            }

            else
            {
                Console.WriteLine("--> We already have data");
            }
        }

        public static void PreparePopulation(IApplicationBuilder app, bool isProduction)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            SeedData(serviceScope.ServiceProvider.GetService<ApplicationDbContext>(), isProduction);
        }
    }
}
