using G3NexusBackend.Data.DTO;
using Microsoft.EntityFrameworkCore;
using G3NexusBackend.Services.Interfaces;

namespace G3NexusBackend.Services;

public class EmployeeService : IEmployeeService
{
    private readonly G3NexusDbContext _context;
    private readonly IEmailService _emailService;

    public EmployeeService(G3NexusDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
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
                ProfileImageUrl = e.ProfileImageUrl,
                IsActive = e.IsActive
            })
            .ToListAsync();
    }

    public async Task<EmployeeDTO?> GetEmployeeByIdAsync(int employeeId)
    {
        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee is not {IsActive: true})
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
            ProfileImageUrl = employee.ProfileImageUrl,
            IsActive = employee.IsActive
        };
    }
    
    public async Task<ApiResponse> CreateEmployeeAsync(EmployeeDTO employeeDto)
    {
        var emailExists = await _context.Employees.AnyAsync(e => e.Email == employeeDto.Email && e.IsActive);
        if (emailExists)
        {
            return new ApiResponse
            {
                Status = false,
                Message = $"An employee with the email '{employeeDto.Email}' already exists."
            };
        }

        var employee = new Employee
        {
            Name = employeeDto.Name,
            ContactNo = employeeDto.ContactNo,
            Email = employeeDto.Email,
            Address = employeeDto.Address,
            Password = BCrypt.Net.BCrypt.HashPassword(employeeDto.Password),
            Role = employeeDto.Role,
            ProfileImageUrl = employeeDto.ProfileImageUrl,
            IsActive = true
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        employeeDto.EmployeeId = employee.EmployeeId;

        var emailTemplate = await _emailService.GetEmailTemplateAsync("PasswordEmailTemplate.html");
        emailTemplate = emailTemplate.Replace("{{Name}}", employeeDto.Name)
            .Replace("{{Password}}", employeeDto.Password);

        await _emailService.SendEmailAsync(employeeDto.Email, "Welcome to G3Nexus", emailTemplate, isHtml: true);

        return new ApiResponse
        {
            Status = true,
            Message = "Employee created successfully.",
            Data = employeeDto
        };
    }
    
    public async Task<EmployeeDTO?> UpdateEmployeeAsync(int employeeId, EmployeeDTO employeeDto)
    {
        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee is not {IsActive: true})
        {
            return null;
        }

        employee.Name = employeeDto.Name;
        employee.ContactNo = employeeDto.ContactNo;
        employee.Address = employeeDto.Address;
        employee.Role = employeeDto.Role;
        employee.ProfileImageUrl = employeeDto.ProfileImageUrl;
        employee.IsActive = employeeDto.IsActive;

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