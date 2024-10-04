using HRSystem.Common.Requests.Employee;
using HRSystem.Common.Responses.Wrapper;
using MediatR;

namespace HRSystem.Application.Features.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommand : IRequest<IResponseWrapper>
{
    public CreateEmployeeRequest CreateEmployeeRequest { get; set; }
}
