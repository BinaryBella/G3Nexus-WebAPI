using G3NexusBackend.DTOs;

namespace G3NexusBackend.Interfaces
{
    public interface IClientService
    {
        Task<IEnumerable<ClientDTO>> GetAllClientsAsync();
        Task<ClientDTO> GetClientByIdAsync(int id);
        Task<ClientDTO> CreateClientAsync(ClientDTO clientDto);
        Task<ClientDTO> UpdateClientAsync(int id, ClientDTO clientDto);
        Task<ApiResponse> DeActivateClientAsync(int id);
    }
}