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

        public async Task<ApiResponse> AddPayment(PaymentDTO paymentDto)
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

            return new ApiResponse { Status = true, Message = "Payment added successfully" };
        }

        public async Task<ApiResponse> EditPayment(PaymentDTO paymentDto)
        {
            var payment = await _context.Payments.FindAsync(paymentDto.PaymentId);
            if (payment == null)
            {
                return new ApiResponse { Status = false, Message = "Payment not found" };
            }

            payment.ProjectId = paymentDto.ProjectId;
            payment.PaymentAmount = paymentDto.PaymentAmount;
            payment.PaymentType = paymentDto.PaymentType;
            payment.PaymentDescription = paymentDto.PaymentDescription;
            payment.PaymentDate = paymentDto.PaymentDate;
            payment.Attachment = paymentDto.Attachment;

            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();

            return new ApiResponse { Status = true, Message = "Payment updated successfully" };
        }

        public async Task<ApiResponse> GetAllPayments()
        {
            var payments = await _context.Payments.ToListAsync();
            return new ApiResponse { Status = true, Data = payments };
        }

        public async Task<ApiResponse> GetPaymentById(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null)
            {
                return new ApiResponse { Status = false, Message = "Payment not found" };
            }

            return new ApiResponse { Status = true, Data = payment };
        }
    }
}
