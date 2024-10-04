using HRSystem.Common.Requests.Employee;
using HRSystem.Common.Responses.Wrapper;
using MediatR;

namespace HRSystem.Application.Features.Employees.Commands.UpdateEmployee
{
    public class UpdateEmployeeCommand : IRequest<IResponseWrapper>
    {
        public UpdateEmployeeRequest UpdateEmployeeRequest { get; set; }
    }
}


