using G3NexusBackend.Data.DTO;
using G3NexusBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace G3NexusBackend.Controllers
{
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

        [HttpGet("{id:int}")]
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
            var response = await _clientService.CreateClientAsync(clientDto);
            if (!response.Status)
            {
                return BadRequest(response);
            }

            return CreatedAtAction(nameof(GetClientById), new { id = ((ClientDTO)response.Data).Id }, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateClient(ClientEditDTO clientDto)
        {
            var client = await _clientService.UpdateClientAsync(clientDto);
            if (client == null)
            {
                return NotFound(new ApiResponse { Status = false, Message = "Client not found" });
            }

            return Ok(new ApiResponse { Status = true, Data = client });
        }

        [HttpDelete("{id:int}")]
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
