using AutoMapper;
using HRSystem.Application.Interfaces;
using HRSystem.Common.Responses.Employee;
using HRSystem.Common.Responses.Wrapper;
using MediatR;

namespace HRSystem.Application.Features.Employees.Queries.GetAllEmployee;

public class GetEmployeesQuery : IRequest<IResponseWrapper>
{
}


public class GetEmployeesHandler(IEmployeeRepository employeeRepository,
    IMapper mapper)
    : IRequestHandler<GetEmployeesQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await employeeRepository.GetAllAsync();
        if (employees.Any())
        {
            var empoloyeeMapped = mapper.Map<List<EmployeeResponse>>(employees);
            return await ResponseWrapper<List<EmployeeResponse>>.SuccessAsync(empoloyeeMapped);
        }

        return await ResponseWrapper.FailAsync("Failed To Retrive Employees");
    }
}
