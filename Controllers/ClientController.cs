using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace G3NexusBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetClients()
        {
            var clients = await _clientService.GetAllClientsAsync();
            return Ok(new ApiResponse { Status = true, Data = clients });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientById(int id)
        {
            var client = await _clientService.GetClientByIdAsync(id);
            if (client == null)
            {
                return NotFound(new ApiResponse { Status = false, Message = "Client not found" });
            }

            return Ok(new ApiResponse { Status = true, Data = client });
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient(ClientDTO clientDto)
        {
            var client = await _clientService.CreateClientAsync(clientDto);
            return CreatedAtAction(nameof(GetClientById), new { id = client.Id }, new ApiResponse { Status = true, Data = client });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, ClientDTO clientDto)
        {
            var client = await _clientService.UpdateClientAsync(id, clientDto);
            if (client == null)
            {
                return NotFound(new ApiResponse { Status = false, Message = "Client not found" });
            }

            return Ok(new ApiResponse { Status = true, Data = client });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeactivateClient(int id)
        {
            var response = await _clientService.DeActivateClientAsync(id);
            if (!response.Status)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}
