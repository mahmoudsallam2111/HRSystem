using HRSystem.Application.Features.Employees.Commands.CreateEmployee;
using HRSystem.Application.Features.Employees.Commands.DeleteEmployee;
using HRSystem.Application.Features.Employees.Commands.UpdateEmployee;
using HRSystem.Application.Features.Employees.Queries.GetAllEmployee;
using HRSystem.Application.Features.Employees.Queries.GetEployee;
using HRSystem.Common.Authorization;
using HRSystem.Common.Requests.Employee;
using HRSystem.WebAPI.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.WebAPI.Controllers
{
    // [Authorize]
    public class EmployeeController : BaseController<EmployeeController>
    {

        [HttpGet]
        [MustHavePermession(AppFeature.Employees, AppAction.Read)]
        public async Task<IActionResult> GetAllEmployee()
        {
            var response = await Sender.Send(new GetEmployeesQuery { });

            if (response.IsSuccessful)
                return Ok(response);

            return NotFound(response);
        }

        [HttpGet("{id}")]
        [MustHavePermession(AppFeature.Employees, AppAction.Read)]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await Sender.Send(new GetEmployeeByIdQuery {EmployeeId = id});

            if (response.IsSuccessful)
               return Ok(response);

            return NotFound(response);
        }


        [HttpPost("create-employee")]
        [MustHavePermession(AppFeature.Employees , AppAction.Read)]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest createEmployeeRequest)
        {
           var response = await Sender.Send(new CreateEmployeeCommand { CreateEmployeeRequest = createEmployeeRequest});

            if (response.IsSuccessful)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpPost("update-employee")]
        [MustHavePermession(AppFeature.Employees, AppAction.Update)]
        public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeRequest  updateEmployeeRequest)
        {
            var response = await Sender.Send(new UpdateEmployeeCommand { UpdateEmployeeRequest = updateEmployeeRequest });
            return Ok(response);
        }

        [HttpDelete("delete-employee/{id}")]
        [MustHavePermession(AppFeature.Employees, AppAction.Delete)]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var response = await Sender.Send(new DeleteEmployeeCommand { EmployeeId = id});
            return Ok(response);
        }


        [HttpGet("highest-salary")]
        public async Task<IActionResult> GetHighestSalary()
        {
            var response = await Sender.Send(new GetHightSalaryEmployeeQuery {  });
            return Ok(response);
        }

    }
}
