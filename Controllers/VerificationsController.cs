using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace G3NexusBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VerificationsController : ControllerBase
    {
        private readonly IVerificationService _verificationService;

        public VerificationsController(IVerificationService verificationService)
        {
            _verificationService = verificationService;
        }

        [HttpPost]
        public async Task<ActionResult> AddVerification(VerificationDTO verificationDto)
        {
            await _verificationService.AddVerificationAsync(verificationDto);
            return Ok(new { message = "Verification code added successfully." });
        }
    }
}