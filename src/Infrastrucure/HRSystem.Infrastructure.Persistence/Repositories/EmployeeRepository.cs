using Ardalis.Specification;
using HRSystem.Application.Interfaces;
using HRSystem.Domain.Entities;
using HRSystem.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Infrastructure.Persistence.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        //public async Task<Employee> GetEmployeeBySpec(ISpecification<Employee> spec)
        //{

        //    return await _dbContext.Set<Employee>()
        //                      .Where(spec.)  // Applying the specification
        //                      .ToListAsync();

        //}
    }
}
