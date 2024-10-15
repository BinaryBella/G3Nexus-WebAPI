using G3NexusBackend.DTOs;

namespace G3NexusBackend.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync();
        Task<EmployeeDTO> GetEmployeeByIdAsync(int employeeId);
        Task<EmployeeDTO> CreateEmployeeAsync(EmployeeDTO employeeDto);
        Task<EmployeeDTO> UpdateEmployeeAsync(int employeeId, EmployeeDTO employeeDto);
        Task<ApiResponse> DeActivateEmployeeAsync(int employeeId);
    }
}