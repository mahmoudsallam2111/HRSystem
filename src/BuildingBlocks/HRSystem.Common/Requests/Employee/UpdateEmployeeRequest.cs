namespace HRSystem.Common.Requests.Employee;

public class UpdateEmployeeRequest
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string FamilyName { get; set; }
    public string Email { get; set; }
    public decimal Salary { get; set; }
}
