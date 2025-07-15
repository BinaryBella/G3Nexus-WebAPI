using G3NexusBackend.Data.DTO;
using G3NexusBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace G3NexusBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(new ApiResponse { Status = true, Data = employees });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound(new ApiResponse { Status = false, Message = "Employee not found" });
            }

            return Ok(new ApiResponse { Status = true, Data = employee });
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(EmployeeDTO employeeDto)
        {
            var response = await _employeeService.CreateEmployeeAsync(employeeDto);
            if (!response.Status)
            {
                return BadRequest(response);
            }

            return CreatedAtAction(nameof(GetEmployeeById), new { id = ((EmployeeDTO)response.Data).EmployeeId }, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEmployee(EmployeeDTO employeeDto)
        {
            var id = employeeDto.EmployeeId;
            var employee = await _employeeService.UpdateEmployeeAsync(id, employeeDto);
            if (employee == null)
            {
                return NotFound(new ApiResponse { Status = false, Message = "Employee not found" });
            }

            return Ok(new ApiResponse { Status = true, Data = employee });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeactivateEmployee(int id)
        {
            var response = await _employeeService.DeActivateEmployeeAsync(id);
            if (!response.Status)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}
