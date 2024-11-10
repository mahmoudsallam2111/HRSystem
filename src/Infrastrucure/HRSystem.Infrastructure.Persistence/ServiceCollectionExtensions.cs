using HRSystem.Application.Interfaces;
using HRSystem.Application.Services.Identity;
using HRSystem.Common.UnitOfWork;
using HRSystem.Infrastructure.Persistence.Context;
using HRSystem.Infrastructure.Persistence.Repositories;
using HRSystem.Infrastructure.Persistence.Repositories.Caching;
using HRSystem.Infrastructure.Persistence.Repositories.Caching.RedisCaching;
using HRSystem.Infrastructure.Persistence.Repositories.CachingRepos;
using HRSystem.Infrastructure.Persistence.Services;
using HRSystem.Infrastructure.Persistence.Services.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;


namespace HRSystem.Infrastructure.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services , IConfiguration configuration)
        {
             services.AddDbContext<ApplicationDbContext>(options =>
             {
                 options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
             })
                .AddTransient<ApplicationDbSeeder>();   // when application start the seeder class is registered

            return services;
        }

        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services.AddTransient<ITokenService, TokenService>();
                //.AddTransient<IUserService, UserService>();  replaced to user scrutor
            return services;
        }

        public static IServiceCollection AddInfrastructureRepositories(this IServiceCollection services)
        {
            // register every class that implement IGenericRepository
       //     RegisterGenericRepositories(services);

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ardalis functionalities
            services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));  // for Ardalis registration
            services.AddScoped(typeof(IReadRepository<>), typeof(CachedRepository<>));    // this is for caching
            services.AddScoped(typeof(MyRepository<>));
            services.AddMemoryCache();

            // configure redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379";
                options.InstanceName = "HRCaching";
                
            });

            return services;
        }

        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddHttpContextAccessor();
            return services;
        }

        private static void RegisterGenericRepositories(IServiceCollection services)
        {
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            var interfaceType = typeof(IGenericRepository<>);
            var interfaces = Assembly.GetAssembly(interfaceType).GetTypes()
                .Where(p => p.GetInterface(interfaceType.Name) != null);

            var implementations = Assembly.GetAssembly(typeof(GenericRepository<>)).GetTypes();

            foreach (var item in interfaces)
            {
                var implementation = implementations.FirstOrDefault(p => p.GetInterface(item.Name) != null);
                services.AddTransient(item, implementation);
            }
        }

        public static void ConfigureInfrastrucureServices(IServiceCollection services)
        {

            var projectDir = @"D:\Mahmoud\HRSystem\src\Infrastrucure\HRSystem.Infrastructure.Persistence";

            // Build the configuration using the path to the directory where appsettings.json is located
            var configuration = new ConfigurationBuilder()
                .SetBasePath(projectDir)  // Correct directory path
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Load email settings from the Infrastructure project's appsettings.json
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            // Other service configurations
            services.Configure<CacheSettings>(configuration.GetSection("CacheSettings"));
        }



    }
}
