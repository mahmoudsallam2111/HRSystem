using HRSystem.Domain.Entities.Employee;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRSystem.Infrastructure.Persistence.EntityTypeConfigurations
{
    internal class EmployeeEntityTypeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees", SchemaNames.Hr);
            builder.HasIndex(e => e.Salary);

            builder.OwnsOne(e => e.FullName , fullName =>
            {
                fullName.Property(f=>f.FirstName).HasColumnName("FirstName");
                fullName.Property(f=>f.SecondName).HasColumnName("SecondName");
                fullName.Property(f=>f.FamilyName).HasColumnName("FamilyName");

                fullName.HasIndex(f => f.FirstName);
                fullName.HasIndex(f => f.FamilyName);
            });
        }
    }
}
