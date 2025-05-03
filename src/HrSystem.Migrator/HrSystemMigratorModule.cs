using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using HRSystem.Infrastructure.Persistence.Context;
using HRSystem.WebAPI;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HrSystem.Migrator
{
    public class HrSystemMigratorModule
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            });

            // Use existing Identity settings from WebAPI
            services.AddIdentitySetting();

            // Register MigrateExecuter and Seeder
            IList<ServiceDescriptor> serviceDescriptors = new List<ServiceDescriptor>()
            {
                ServiceDescriptor.Transient<IMigrateExecuter, MigrateExecuter>(),
                ServiceDescriptor.Transient<IApplicationDbSeeder, ApplicationDbSeeder>()
            };

            foreach (var serviceDescriptor in serviceDescriptors)
            {
                services.Add(serviceDescriptor);
            }
        }
    }
} 