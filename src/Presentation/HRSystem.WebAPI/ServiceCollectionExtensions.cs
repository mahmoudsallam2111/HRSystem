using HRSystem.Infrastructure.Persistence.Context;
using HRSystem.Infrastructure.Persistence.Models;

namespace HRSystem.WebAPI
{
    public static class ServiceCollectionExtensions
    {

        internal static IServiceCollection AddIdentitySettings(this ServiceCollection services)
        {
            services.AddIdentity<ApplicationUser , ApplicationRole>(opt =>
            {
                opt.Password.RequiredLength = 6;
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
        }


        internal static IApplicationBuilder SeedDataBase(this IApplicationBuilder app)
        {
            // create a scope
           using var serviceScpoe =  app.ApplicationServices.CreateScope();

            var seeders = serviceScpoe.ServiceProvider.GetServices<ApplicationDbSeeder>();

            foreach (var seeder in seeders)
            {
                seeder.SeedDatabaseAsync().GetAwaiter().GetResult();
            }

            return app;
        }
    }
}
