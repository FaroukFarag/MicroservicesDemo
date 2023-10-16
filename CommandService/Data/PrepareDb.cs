using CommandService.Models;
using CommandService.SyncDataServices.Grpc;

namespace CommandService.Data
{
    public static class PrepareDb
    {
        private static void SeedData(ICommandRepository repository, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("--> Seeding new Platforms...");

            foreach (var platform in platforms)
            {
                if(!repository.ExternalPlatformExists(platform.ExternalId))
                {
                    repository.CreatePlatform(platform);
                }

                repository.SaveChanges();
            }
        }

        public static void PreparePopulation(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var grpcClient = serviceScope.ServiceProvider.GetRequiredService<IPlatformDataClient>();
            var platforms = grpcClient.ReturnAllPlatforms();

            SeedData(serviceScope.ServiceProvider.GetService<ICommandRepository>(), platforms);
        }
    }
}
