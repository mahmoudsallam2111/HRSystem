using Ardalis.Specification;
using HRSystem.Domain.Entities;
using System.Xml.Linq;

namespace HRSystem.Application.Specifications
{
    public class EmployeeWithGreatestSalarySpecification : Specification<Employee>
    {
        public EmployeeWithGreatestSalarySpecification()
        {
            Query
                .OrderByDescending(x => x.Salary)
                .EnableCache(nameof(EmployeeWithGreatestSalarySpecification));
        }
    }
}
