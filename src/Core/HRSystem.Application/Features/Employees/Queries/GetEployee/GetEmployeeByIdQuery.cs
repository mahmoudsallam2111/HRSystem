using AutoMapper;
using HRSystem.Application.Interfaces;
using HRSystem.Common.Responses.Employee;
using HRSystem.Common.Responses.Wrapper;
using MediatR;

namespace HRSystem.Application.Features.Employees.Queries.GetEployee;

public class GetEmployeeByIdQuery : IRequest<IResponseWrapper>
{
    public int EmployeeId { get; set; }
}


public class GetEmployeeByIdHandler : IRequestHandler<GetEmployeeByIdQuery, IResponseWrapper>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;

    public GetEmployeeByIdHandler(IEmployeeRepository employeeRepository,
        IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }
    public async Task<IResponseWrapper> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
        if (employee is not null)
        {
            var empoloyeeMapped = _mapper.Map<EmployeeResponse>(employee);
            return await ResponseWrapper<EmployeeResponse>.SuccessAsync(empoloyeeMapped);
        }

        return await ResponseWrapper.FailAsync("Failed To Get Employee");
    }
}
