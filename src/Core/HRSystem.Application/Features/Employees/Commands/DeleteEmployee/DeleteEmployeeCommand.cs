using HRSystem.Application.Interfaces;
using HRSystem.Common.Responses.Wrapper;
using HRSystem.Common.UnitOfWork;
using MediatR;

namespace HRSystem.Application.Features.Employees.Commands.DeleteEmployee;

public class DeleteEmployeeCommand : IRequest<IResponseWrapper>
{
    public int EmployeeId { get; set; }
}


public class DeleteEmployeeHandler : IRequestHandler<DeleteEmployeeCommand, IResponseWrapper>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEmployeeHandler(IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<IResponseWrapper> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
        if (employee == null)
            return await ResponseWrapper<int>.FailAsync("Employee does not exists");

        await _employeeRepository.DeleteAsync(employee);

        var response = await _unitOfWork.SaveChangesAsync();

        if (response > 0)
        {
            return await ResponseWrapper<int>.SuccessAsync("Employee Deleted Successfully");
        };

        return await ResponseWrapper<int>.FailAsync("failed to Delete Employee");
    }
}
