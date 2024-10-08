namespace HRSystem.Common.Requests.Identity
{
    public class UserRegistrationRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumer { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool ActivateUser { get; set; }
        public bool AutoConfirmEmail { get; set; }
    }
}
