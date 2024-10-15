using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetPayments()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return Ok(new ApiResponse { Status = true, Data = payments });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound(new ApiResponse { Status = false, Message = "Payment not found" });
            }

            return Ok(new ApiResponse { Status = true, Data = payment });
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(PaymentDTO paymentDto)
        {
            var payment = await _paymentService.CreatePaymentAsync(paymentDto);
            return CreatedAtAction(nameof(GetPaymentById), new { id = payment.PaymentId }, new ApiResponse { Status = true, Data = payment });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, PaymentDTO paymentDto)
        {
            var payment = await _paymentService.UpdatePaymentAsync(id, paymentDto);
            if (payment == null)
            {
                return NotFound(new ApiResponse { Status = false, Message = "Payment not found" });
            }

            return Ok(new ApiResponse { Status = true, Data = payment });
        }

        [HttpDelete("{id}")]
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
