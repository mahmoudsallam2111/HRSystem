using AutoMapper;
using HRSystem.Common.Requests.Employee;
using HRSystem.Common.Responses.Employee;
using HRSystem.Domain.Entities;
using HRSystem.Domain.Entities.ValueObjects;

namespace HRSystem.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, CreateEmployeeRequest>().ReverseMap()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => new FullName
                {
                    FirstName = src.FirstName,
                    SecondName = src.SecondName,
                    FamilyName = src.FamilyName,
                }));

            CreateMap<EmployeeResponse, Employee>().ReverseMap()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FullName.FirstName))
                .ForMember(dest => dest.SecondName, opt => opt.MapFrom(src => src.FullName.SecondName))
                .ForMember(dest => dest.FamilyName, opt => opt.MapFrom(src => src.FullName.FamilyName));
        }
    }
}
