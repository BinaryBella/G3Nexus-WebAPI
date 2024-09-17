using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace G3NexusBackend.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly G3NexusDbContext _context;

        public PaymentService(G3NexusDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PaymentDTO>> GetAllPaymentsAsync()
        {
            return await _context.Payments.Select(payment => new PaymentDTO
            {
                PaymentId = payment.PaymentId,
                ProjectId = payment.ProjectId,
                PaymentAmount = payment.PaymentAmount,
                PaymentType = payment.PaymentType,
                PaymentDescription = payment.PaymentDescription,
                PaymentDate = payment.PaymentDate,
                Attachment = payment.Attachment
            }).ToListAsync();
        }

        public async Task<PaymentDTO> GetPaymentByIdAsync(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null) return null;

            return new PaymentDTO
            {
                PaymentId = payment.PaymentId,
                ProjectId = payment.ProjectId,
                PaymentAmount = payment.PaymentAmount,
                PaymentType = payment.PaymentType,
                PaymentDescription = payment.PaymentDescription,
                PaymentDate = payment.PaymentDate,
                Attachment = payment.Attachment
            };
        }

        public async Task AddPaymentAsync(PaymentDTO paymentDto)
        {
            var payment = new Payment
            {
                ProjectId = paymentDto.ProjectId,
                PaymentAmount = paymentDto.PaymentAmount,
                PaymentType = paymentDto.PaymentType,
                PaymentDescription = paymentDto.PaymentDescription,
                PaymentDate = paymentDto.PaymentDate,
                Attachment = paymentDto.Attachment
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePaymentAsync(int paymentId, PaymentDTO paymentDto)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null) return;

            payment.ProjectId = paymentDto.ProjectId;
            payment.PaymentAmount = paymentDto.PaymentAmount;
            payment.PaymentType = paymentDto.PaymentType;
            payment.PaymentDescription = paymentDto.PaymentDescription;
            payment.PaymentDate = paymentDto.PaymentDate;
            payment.Attachment = paymentDto.Attachment;

            await _context.SaveChangesAsync();
        }
    }
}
