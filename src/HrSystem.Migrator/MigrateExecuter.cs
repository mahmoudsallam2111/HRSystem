using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using HRSystem.Infrastructure.Persistence.Context;
using HrSystem.Shared.SQLScripts;

namespace HrSystem.Migrator
{
    public class MigrateExecuter : IMigrateExecuter
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MigrateExecuter> _logger;
        private readonly ApplicationDbContext _context;

        public MigrateExecuter(
            IConfiguration configuration,
            ILogger<MigrateExecuter> logger,
            ApplicationDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        public async Task RunAsync()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");

                _logger.LogInformation("Starting database migration...");
                
                // Run Entity Framework migrations
                await _context.Database.MigrateAsync();
                _logger.LogInformation("Entity Framework migrations completed successfully.");

                // Run SQL scripts
                 _logger.LogInformation("Starting SQL scripts deployment...");
                Deploy.Upgrade(connectionString!, _logger);
                _logger.LogInformation("SQL scripts deployment completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while migrating the database.");
                throw;
            }
        }
    }
}