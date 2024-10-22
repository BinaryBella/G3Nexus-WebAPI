using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace G3NexusBackend.Services
{
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
                    OrganizationName = c.OrganizationName,
                    Name = c.Name,
                    ContactNo = c.ContactNo,
                    Email = c.Email,
                    Address = c.Address,
                    Role = c.Role,
                    IsActive = c.IsActive
                })
                .ToListAsync();
        }

        public async Task<ClientDTO> GetClientByIdAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null || !client.IsActive)
            {
                return null;
            }

            return new ClientDTO
            {
                Id = client.Id,
                OrganizationName = client.OrganizationName,
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
            var client = new Client
            {
                OrganizationName = clientDto.OrganizationName,
                Name = clientDto.Name,
                ContactNo = clientDto.ContactNo,
                Email = clientDto.Email,
                Address = clientDto.Address,
                Password = BCrypt.Net.BCrypt.HashPassword(clientDto.Password),
                Role = clientDto.Role,
                IsActive = true
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            clientDto.Id = client.Id;
            return clientDto;
        }

        public async Task<ClientDTO> UpdateClientAsync(int id, ClientDTO clientDto)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null || !client.IsActive)
            {
                return null;
            }

            client.Name = clientDto.Name;
            client.ContactNo = clientDto.ContactNo;
            client.Email = clientDto.Email;
            client.Address = clientDto.Address;

            if (!string.IsNullOrEmpty(clientDto.Password))
            {
                client.Password = BCrypt.Net.BCrypt.HashPassword(clientDto.Password);
            }

            _context.Clients.Update(client);
            await _context.SaveChangesAsync();

            return clientDto;
        }

        public async Task<ApiResponse> DeActivateClientAsync(int Id)
        {
            var client = await _context.Clients.FindAsync(Id);
            if (client == null || !client.IsActive)
            {
                return new ApiResponse { Status = false, Message = "Client not found or already inactive." };
            }

            client.IsActive = false;
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();

            return new ApiResponse { Status = true, Message = "Client successfully deactivated." };
        }
    }
}
