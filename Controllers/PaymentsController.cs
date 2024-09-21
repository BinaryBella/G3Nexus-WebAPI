using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace G3NexusBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddPayment([FromBody] PaymentDTO paymentDto)
        {
            var response = await _paymentService.AddPayment(paymentDto);
            return Ok(response);
        }

        [HttpPut("edit")]
        public async Task<IActionResult> EditPayment([FromBody] PaymentDTO paymentDto)
        {
            var response = await _paymentService.EditPayment(paymentDto);
            return Ok(response);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllPayments()
        {
            var response = await _paymentService.GetAllPayments();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var response = await _paymentService.GetPaymentById(id);
            return Ok(response);
        }
    }
}