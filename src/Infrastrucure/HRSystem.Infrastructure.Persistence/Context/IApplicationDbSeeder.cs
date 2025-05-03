namespace HRSystem.Infrastructure.Persistence.Context
{
    public interface IApplicationDbSeeder
    {
        public Task SeedDatabaseAsync();
    }
}