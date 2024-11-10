using HRSystem.Application.Interfaces;
using HRSystem.Application.Specifications;
using HRSystem.Common.Responses.Wrapper;
using HRSystem.Domain.Entities;
using MediatR;

namespace HRSystem.Application.Features.Employees.Queries.GetEployee;

public class GetHightSalaryEmployeeQuery  : IRequest<IResponseWrapper>
{
}


public class GetHightSalaryEmployeeHandler : IRequestHandler<GetHightSalaryEmployeeQuery, IResponseWrapper>
{
    private readonly IReadRepository<Employee> _readRepositoryBase;

    public GetHightSalaryEmployeeHandler(IReadRepository<Employee> readRepositoryBase)
    {
        _readRepositoryBase = readRepositoryBase;
    }
    public async Task<IResponseWrapper> Handle(GetHightSalaryEmployeeQuery request, CancellationToken cancellationToken)
    {
        var spac = new EmployeeWithGreatestSalarySpecification();
        var employee =await _readRepositoryBase.FirstOrDefaultAsync(spac);
        return await ResponseWrapper<Employee?>.SuccessAsync(employee);
    }
}

