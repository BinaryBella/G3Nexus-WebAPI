using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace G3NexusBackend.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly G3NexusDbContext _context;

        public EmployeeService(G3NexusDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Where(e => e.IsActive) // Only get active employees
                .Select(e => new EmployeeDTO
                {
                    EmployeeId = e.EmployeeId,
                    Name = e.Name,
                    ContactNo = e.ContactNo,
                    Email = e.Email,
                    Address = e.Address,
                    Role = e.Role,
                    IsActive = e.IsActive
                })
                .ToListAsync();
        }

        public async Task<EmployeeDTO> GetEmployeeByIdAsync(int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null || !employee.IsActive)
            {
                return null;
            }

            return new EmployeeDTO
            {
                EmployeeId = employee.EmployeeId,
                Name = employee.Name,
                ContactNo = employee.ContactNo,
                Email = employee.Email,
                Address = employee.Address,
                Role = employee.Role,
                IsActive = employee.IsActive
            };
        }

        public async Task<EmployeeDTO> CreateEmployeeAsync(EmployeeDTO employeeDto)
        {
            var employee = new Employee
            {
                Name = employeeDto.Name,
                ContactNo = employeeDto.ContactNo,
                Email = employeeDto.Email,
                Address = employeeDto.Address,
                Password = BCrypt.Net.BCrypt.HashPassword(employeeDto.Password),
                Role = employeeDto.Role,
                IsActive = true
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            employeeDto.EmployeeId = employee.EmployeeId;
            return employeeDto;
        }

        public async Task<EmployeeDTO> UpdateEmployeeAsync(int employeeId, EmployeeDTO employeeDto)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null || !employee.IsActive)
            {
                return null;
            }

            employee.Name = employeeDto.Name;
            employee.ContactNo = employeeDto.ContactNo;
            employee.Email = employeeDto.Email;
            employee.Address = employeeDto.Address;

            if (!string.IsNullOrEmpty(employeeDto.Password))
            {
                employee.Password = BCrypt.Net.BCrypt.HashPassword(employeeDto.Password);
            }

            employee.Role = employeeDto.Role;

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            return employeeDto;
        }

        public async Task<ApiResponse> DeActivateEmployeeAsync(int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null || !employee.IsActive)
            {
                return new ApiResponse { Status = false, Message = "Employee not found or already inactive." };
            }

            employee.IsActive = false;
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            return new ApiResponse { Status = true, Message = "Employee successfully deactivated." };
        }
    }
}
