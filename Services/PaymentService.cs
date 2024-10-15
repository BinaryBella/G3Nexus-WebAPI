using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using G3NexusBackend.Models;
using Microsoft.EntityFrameworkCore;

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
            return await _context.Payments
                .Where(p => p.IsActive) // Only return active payments
                .Select(p => new PaymentDTO
                {
                    PaymentId = p.PaymentId,
                    ProjectId = p.ProjectId,
                    PaymentAmount = p.PaymentAmount,
                    PaymentType = p.PaymentType,
                    PaymentDescription = p.PaymentDescription,
                    PaymentDate = p.PaymentDate,
                    Attachment = p.Attachment,
                    IsActive = p.IsActive
                })
                .ToListAsync();
        }

        public async Task<PaymentDTO> GetPaymentByIdAsync(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null || !payment.IsActive)
            {
                return null;
            }

            return new PaymentDTO
            {
                PaymentId = payment.PaymentId,
                ProjectId = payment.ProjectId,
                PaymentAmount = payment.PaymentAmount,
                PaymentType = payment.PaymentType,
                PaymentDescription = payment.PaymentDescription,
                PaymentDate = payment.PaymentDate,
                Attachment = payment.Attachment,
                IsActive = payment.IsActive
            };
        }

        public async Task<PaymentDTO> CreatePaymentAsync(PaymentDTO paymentDto)
        {
            var payment = new Payment
            {
                ProjectId = paymentDto.ProjectId,
                PaymentAmount = paymentDto.PaymentAmount,
                PaymentType = paymentDto.PaymentType,
                PaymentDescription = paymentDto.PaymentDescription,
                PaymentDate = paymentDto.PaymentDate,
                Attachment = paymentDto.Attachment,
                IsActive = true // New payments are active by default
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            paymentDto.PaymentId = payment.PaymentId;
            return paymentDto;
        }

        public async Task<PaymentDTO> UpdatePaymentAsync(int paymentId, PaymentDTO paymentDto)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null || !payment.IsActive)
            {
                return null;
            }

            payment.ProjectId = paymentDto.ProjectId;
            payment.PaymentAmount = paymentDto.PaymentAmount;
            payment.PaymentType = paymentDto.PaymentType;
            payment.PaymentDescription = paymentDto.PaymentDescription;
            payment.PaymentDate = paymentDto.PaymentDate;
            payment.Attachment = paymentDto.Attachment;

            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();

            return paymentDto;
        }

        public async Task<ApiResponse> DeActivatePaymentAsync(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null || !payment.IsActive)
            {
                return new ApiResponse { Status = false, Message = "Payment not found or already inactive." };
            }

            payment.IsActive = false;
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();

            return new ApiResponse { Status = true, Message = "Payment successfully deactivated." };
        }
    }
}
