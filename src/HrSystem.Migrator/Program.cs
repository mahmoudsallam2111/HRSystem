using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using HRSystem.Infrastructure.Persistence.Context;
using HrSystem.Shared.SQLScripts;
using Microsoft.Extensions.DependencyInjection;

namespace HrSystem.Migrator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    HrSystemMigratorModule.ConfigureServices(services, hostContext.Configuration);
                    
                })
                .Build();

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            var seeders = scope.ServiceProvider.GetServices<IApplicationDbSeeder>();

            // now u can remove it from the program.cs class in the web project 
            // also we can replace it with sql script
            foreach (var seeder in seeders)      
            {
                await seeder.SeedDatabaseAsync();
            }

            try
            {
                var migrateExecuter = services.GetRequiredService<IMigrateExecuter>();
                await migrateExecuter.RunAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while running the migrator.");
                throw;
            }
        }
    }
}
