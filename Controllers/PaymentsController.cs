using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace G3NexusBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDTO>>> GetAllPayments()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return Ok(payments);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDTO>> GetPaymentById(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null) return NotFound();

            return Ok(payment);
        }

        [HttpPost]
        public async Task<ActionResult> AddPayment(PaymentDTO paymentDto)
        {
            await _paymentService.AddPaymentAsync(paymentDto);
            return CreatedAtAction(nameof(GetPaymentById), new { id = paymentDto.PaymentId }, paymentDto);
        }

        
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePayment(int id, PaymentDTO paymentDto)
        {
            await _paymentService.UpdatePaymentAsync(id, paymentDto);
            return NoContent();
        }
    }
}