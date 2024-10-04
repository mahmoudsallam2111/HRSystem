using HRSystem.Domain.Entities.ValueObjects;

namespace HRSystem.Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public FullName FullName { get; set; }
        public string Email { get; set; }
        public decimal Salary { get; set; }

    }
}
