namespace HRSystem.Common.Requests.Employee
{
    public class CreateEmployeeRequest
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FamilyName { get; set; } 
        public string Email { get; set; }
        public decimal Salary { get; set; }
    }
}
