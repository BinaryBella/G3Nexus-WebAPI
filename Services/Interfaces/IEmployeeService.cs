using G3NexusBackend.Data.DTO;

namespace G3NexusBackend.Services.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync();
    Task<EmployeeDTO?> GetEmployeeByIdAsync(int employeeId);
    Task<EmployeeDTO> CreateEmployeeAsync(EmployeeDTO employeeDto);
    Task<EmployeeDTO?> UpdateEmployeeAsync(int employeeId, EmployeeDTO employeeDto);
    Task<ApiResponse> DeActivateEmployeeAsync(int employeeId);
}