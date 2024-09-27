using System.ComponentModel.DataAnnotations.Schema;

namespace HRSystem.Domain.Entities.ValueObjects
{
    public class FullName
    {
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; }
        public string FamilyName { get; set; } = string.Empty;
    }
}
