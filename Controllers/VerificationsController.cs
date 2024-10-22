using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace G3NexusBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VerificationController : ControllerBase
    {
        private readonly IVerificationService _verificationService;

        public VerificationController(IVerificationService verificationService)
        {
            _verificationService = verificationService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddVerification([FromBody] VerificationDTO verificationDto)
        {
            var response = await _verificationService.AddVerification(verificationDto);
            return Ok(response);
        }
    }

}