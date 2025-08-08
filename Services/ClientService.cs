using G3NexusBackend.Data.DTO;
using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using G3NexusBackend.Services.Interfaces;

namespace G3NexusBackend.Services;

public class ClientService : IClientService
{
    private readonly G3NexusDbContext _context;
    private readonly IEmailService _emailService;

    public ClientService(G3NexusDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<IEnumerable<ClientDTO>> GetAllClientsAsync()
    {
        return await _context.Clients
            .Where(c => c.IsActive) // Only get active clients
            .Select(c => new ClientDTO
            {
                Id = c.Id,
                Name = c.Name,
                ContactNo = c.ContactNo,
                Email = c.Email,
                Address = c.Address,
                Role = c.Role,
                IsActive = c.IsActive,
                CompanyId = c.CompanyId
            })
            .ToListAsync();
    }

    public async Task<ClientDTO?> GetClientByIdAsync(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client is not {IsActive: true})
        {
            return null;
        }

        return new ClientDTO
        {
            Id = client.Id,
            Name = client.Name,
            ContactNo = client.ContactNo,
            Email = client.Email,
            Address = client.Address,
            Role = client.Role,
            IsActive = client.IsActive,
            CompanyId = client.CompanyId
        };
    }

    public async Task<ApiResponse> CreateClientAsync(ClientDTO clientDto)
    {
        // Check if a client with the same email already exists
        var emailExists = await _context.Clients.AnyAsync(c => c.Email == clientDto.Email && c.IsActive);
        if (emailExists)
        {
            return new ApiResponse
            {
                Status = false,
                Message = $"A client with the email '{clientDto.Email}' already exists."
            };
        }

        // Check if the associated company exists
        var companyExists = await _context.Companies.AnyAsync(c => c.CompanyId == clientDto.CompanyId && c.IsActive);
        if (!companyExists)
        {
            return new ApiResponse
            {
                Status = false,
                Message = $"Company with ID {clientDto.CompanyId} not found."
            };
        }

        var client = new Client
        {
            Name = clientDto.Name,
            ContactNo = clientDto.ContactNo,
            Email = clientDto.Email,
            Address = clientDto.Address,
            Password = BCrypt.Net.BCrypt.HashPassword(clientDto.Password),
            Role = clientDto.Role,
            IsActive = true,
            CompanyId = clientDto.CompanyId
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        clientDto.Id = client.Id;
        
        var emailTemplate = await _emailService.GetEmailTemplateAsync("PasswordEmailTemplate.html");
        emailTemplate = emailTemplate.Replace("{{Name}}", clientDto.Name)
            .Replace("{{Password}}", clientDto.Password);

        await _emailService.SendEmailAsync(clientDto.Email, "Welcome to G3Nexus", emailTemplate, isHtml: true);

        return new ApiResponse
        {
            Status = true,
            Message = "Client created successfully.",
            Data = clientDto
        };
    }
    public async Task<ClientEditDTO?> UpdateClientAsync(ClientEditDTO clientDto)
    {
        var client = await _context.Clients.FindAsync(clientDto.Id);
        if (client is not {IsActive: true})
        {
            return null;
        }

        client.Name = clientDto.Name;
        client.ContactNo = clientDto.ContactNo;
        client.Email = clientDto.Email;
        client.Address = clientDto.Address;
        client.ProfileImageUrl = clientDto.ProfileImageUrl;
        client.Role = clientDto.Role;
        _context.Clients.Update(client);
        await _context.SaveChangesAsync();

        return clientDto;
    }

    public async Task<ApiResponse> DeActivateClientAsync(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client is not {IsActive: true})
        {
            throw new KeyNotFoundException("Client not found or already inactive.");
        }

        var clientRequirements = await _context.Requirements.Where(r => r.ClientId == client.Id).Select(r => r.RequirementTitle).ToListAsync();
        if (clientRequirements.Any())
        {
            return new ApiResponse
            {
                Status = false,
                Message = "Cannot deactivate client with active requirements. Please delete the following requirements first.",
                Data = clientRequirements
            };
        }

        var clientBugs = await _context.Bugs.Where(r => r.ClientId == client.Id).Select(r => r.BugTitle).ToListAsync();
        if (clientBugs.Any())
        {
            return new ApiResponse
            {
                Status = false,
                Message = "Cannot deactivate client with active bugs. Please delete the following bugs first.",
                Data = clientBugs
            };
        }

        client.IsActive = false;
        _context.Clients.Update(client);
        await _context.SaveChangesAsync();

        return new ApiResponse { Status = true, Message = "Client successfully deactivated." };
    }
    
    public async Task<ClientDTO?> GetClientAdminByCompanyIdAsync(int companyId)
    {
        return await _context.Clients
            .Where(c => c.CompanyId == companyId &&
                        c.Role == "CLIENT_ADMIN" &&
                        c.IsActive)
            .Select(c => new ClientDTO
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                ContactNo = c.ContactNo,
                Address = c.Address,
                Role = c.Role,
                CompanyId = c.CompanyId,
                IsActive = c.IsActive
            })
            .FirstOrDefaultAsync();
    }

}