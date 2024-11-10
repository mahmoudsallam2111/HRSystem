using Ardalis.Specification;
using HRSystem.Common.DIContracts;
using HRSystem.Domain.Entities;

namespace HRSystem.Application.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employee> , IScopedService
    {
    }
}
