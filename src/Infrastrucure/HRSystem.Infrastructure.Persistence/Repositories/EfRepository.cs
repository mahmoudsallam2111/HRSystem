using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using HRSystem.Application.Interfaces;
using HRSystem.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Infrastructure.Persistence.Repositories
{
    public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T> where T : class
    {
        public EfRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
