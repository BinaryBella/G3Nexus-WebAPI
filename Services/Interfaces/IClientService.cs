using G3NexusBackend.Data.DTO;

namespace G3NexusBackend.Services.Interfaces;

public interface IClientService
{
    Task<IEnumerable<ClientDTO>> GetAllClientsAsync();
    Task<ClientDTO?> GetClientByIdAsync(int id);
    Task<ClientDTO> CreateClientAsync(ClientDTO clientDto);
    Task<ClientDTO?> UpdateClientAsync(ClientDTO clientDto);
    Task<ApiResponse> DeActivateClientAsync(int id);
}