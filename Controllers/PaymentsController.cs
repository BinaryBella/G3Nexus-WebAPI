using G3NexusBackend.Data.DTO;
using G3NexusBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace G3NexusBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

    [HttpGet]
    [Authorize(Roles = "CLIENT_ADMIN,COMPANY_ADMIN")]
    public async Task<IActionResult> GetPayments()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return Ok(new ApiResponse { Status = true, Data = payments });
        }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "CLIENT_ADMIN,COMPANY_ADMIN")]
    public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound(new ApiResponse { Status = false, Message = "Payment not found" });
            }

            return Ok(new ApiResponse { Status = true, Data = payment });
        }

    [HttpGet("client/{clientId:int}")]
    [Authorize(Roles = "CLIENT_ADMIN,COMPANY_ADMIN")]
    public async Task<IActionResult> GetPaymentsByClientId(int clientId)
        {
            var payments = await _paymentService.GetPaymentsByClientIdAsync(clientId);
            return Ok(new ApiResponse { Status = true, Data = payments });
        }

    [HttpPost]
    [Authorize(Roles = "CLIENT_ADMIN")]
    public async Task<IActionResult> CreatePayment(PaymentDTO paymentDto)
        {
            try
            {
                var payment = await _paymentService.CreatePaymentAsync(paymentDto);
                return CreatedAtAction(nameof(GetPaymentById), new { id = payment.PaymentId }, new ApiResponse { Status = true, Data = payment });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new ApiResponse { Status = false, Message = ex.Message });
            }
        }

        
    [HttpPut]
    [Authorize(Roles = "CLIENT_ADMIN,COMPANY_ADMIN")]
    public async Task<IActionResult> UpdatePayment(PaymentDTO paymentDto)
        {
            try
            {
                var id = paymentDto.PaymentId;
                var payment = await _paymentService.UpdatePaymentAsync(id, paymentDto);
                if (payment == null)
                {
                    return NotFound(new ApiResponse { Status = false, Message = "Payment not found" });
                }

                return Ok(new ApiResponse { Status = true, Data = payment });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new ApiResponse { Status = false, Message = ex.Message });
            }

        }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    public async Task<IActionResult> DeactivatePayment(int id)
        {
            var response = await _paymentService.DeActivatePaymentAsync(id);
            if (!response.Status)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}
