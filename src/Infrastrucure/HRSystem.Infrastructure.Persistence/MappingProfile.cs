using AutoMapper;
using HRSystem.Common.Responses.Identity;
using HRSystem.Infrastructure.Persistence.Models;

namespace HRSystem.Infrastructure.Persistence
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser?, UserRegistrationResponse>().ReverseMap();
            CreateMap<ApplicationRole?, RoleResponse>().ReverseMap();
            CreateMap<ApplicationRoleClaim?, RoleClaimViewModel>().ReverseMap();
        }
    }
}
