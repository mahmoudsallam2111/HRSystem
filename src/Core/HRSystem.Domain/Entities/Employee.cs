using HRSystem.Domain.Entities.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace HRSystem.Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public FullName FullName { get; set; }
        public string Email { get; set; }
        public decimal Salary { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; } // RowVersion column for concurrency

        //public int MyProperty { get; set; }

    }
}
