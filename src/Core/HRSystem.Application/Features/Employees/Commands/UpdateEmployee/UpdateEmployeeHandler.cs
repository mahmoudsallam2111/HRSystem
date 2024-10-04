using AutoMapper;
using HRSystem.Application.Interfaces;
using HRSystem.Common.Responses.Wrapper;
using HRSystem.Common.UnitOfWork;
using MediatR;

namespace HRSystem.Application.Features.Employees.Commands.UpdateEmployee
{
    public class UpdateEmployeeHandler : IRequestHandler<UpdateEmployeeCommand, IResponseWrapper>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateEmployeeHandler(IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IResponseWrapper> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.UpdateEmployeeRequest.Id);
            if (employee is null)
            {
                return await ResponseWrapper.FailAsync("Employee does not exist");
            }

            employee.FullName = new Domain.Entities.ValueObjects.FullName
            {
                FirstName = request.UpdateEmployeeRequest.FirstName,
                SecondName = request.UpdateEmployeeRequest.SecondName,
                FamilyName = request.UpdateEmployeeRequest.FamilyName,
            };

            employee.Salary = request.UpdateEmployeeRequest.Salary;
            employee.Email = request.UpdateEmployeeRequest.Email;

            var response = await _unitOfWork.SaveChangesAsync();
            if (response > 0)
            {
                return await ResponseWrapper<int>.SuccessAsync("Employee Updated Successfully");
            };

            return await ResponseWrapper<int>.FailAsync("failed to Update Employee");


        }
    }
}
