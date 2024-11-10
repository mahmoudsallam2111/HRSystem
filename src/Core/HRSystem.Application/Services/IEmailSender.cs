using HRSystem.Common.DIContracts;

namespace HRSystem.Application.Services
{
    public interface IEmailSender : ITransiantService
    {
        public Task SendEmailAsync(string email , string subject , string message);
    }
}
