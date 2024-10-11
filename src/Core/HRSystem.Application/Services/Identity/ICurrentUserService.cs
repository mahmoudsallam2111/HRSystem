using HRSystem.Common.DIContracts;

namespace HRSystem.Application.Services.Identity
{
    public interface ICurrentUserService : IScopedService
    {
        public string UserId { get; }
    }
}
