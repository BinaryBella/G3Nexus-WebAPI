using G3NexusBackend.Data.DTO;
using Microsoft.EntityFrameworkCore;
using G3NexusBackend.Services.Interfaces;

namespace G3NexusBackend.Services;

public class ClientService : IClientService
{
    private readonly G3NexusDbContext _context;

    public ClientService(G3NexusDbContext context)
    {
        _context = context;
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
                IsActive = c.IsActive
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
            IsActive = client.IsActive
        };
    }

    public async Task<ClientDTO> CreateClientAsync(ClientDTO clientDto)
    {
        var companyExists = await _context.Companies.AnyAsync(c => c.CompanyId == clientDto.CompanyId && c.IsActive);
        if (!companyExists)
        {
            throw new KeyNotFoundException($"Company with ID {clientDto.CompanyId} not found.");
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
        return clientDto;
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
        _context.Clients.Update(client);
        await _context.SaveChangesAsync();

        return clientDto;
    }

    public async Task<ApiResponse> DeActivateClientAsync(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client is not {IsActive: true})
        {
            return new ApiResponse { Status = false, Message = "Client not found or already inactive." };
        }

        client.IsActive = false;
        _context.Clients.Update(client);
        await _context.SaveChangesAsync();

        return new ApiResponse { Status = true, Message = "Client successfully deactivated." };
    }
}