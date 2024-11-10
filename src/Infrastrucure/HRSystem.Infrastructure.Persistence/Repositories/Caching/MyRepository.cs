using Ardalis.Specification.EntityFrameworkCore;
using HRSystem.Application.Interfaces;
using HRSystem.Infrastructure.Persistence.Context;

namespace HRSystem.Infrastructure.Persistence.Repositories.CachingRepos
{
    public class MyRepository<T> : RepositoryBase<T>, IRepository<T> where T : class
    {
        private readonly ApplicationDbContext dbContext;

        public MyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        // Not required to implement anything. Add additional functionalities if required.
    }
}
