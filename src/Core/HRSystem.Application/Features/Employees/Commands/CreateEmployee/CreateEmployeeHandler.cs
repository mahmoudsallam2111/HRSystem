using AutoMapper;
using HRSystem.Application.Interfaces;
using HRSystem.Common.Responses.Wrapper;
using HRSystem.Common.UnitOfWork;
using HRSystem.Domain.Entities;
using MediatR;

namespace HRSystem.Application.Features.Employees.Commands.CreateEmployee
{
    public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, IResponseWrapper>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CreateEmployeeHandler(IEmployeeRepository employeeRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<IResponseWrapper> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = _mapper.Map<Employee>(request.CreateEmployeeRequest);
           await _employeeRepository.AddAsync(employee);
           var response =  await _unitOfWork.SaveChangesAsync();

            if (response > 0)
            {
                return await ResponseWrapper<int>.SuccessAsync("Employee Created Successfully");
            };

            return await ResponseWrapper<int>.FailAsync("failed to Create Employee");

        }
    }
}
