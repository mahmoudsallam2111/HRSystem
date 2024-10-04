using HRSystem.Application.Interfaces;
using HRSystem.Domain.Entities;
using HRSystem.Infrastructure.Persistence.Context;

namespace HRSystem.Infrastructure.Persistence.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
