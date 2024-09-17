using G3NexusBackend.DTOs;

namespace G3NexusBackend.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDTO>> GetAllPaymentsAsync();
        Task<PaymentDTO> GetPaymentByIdAsync(int paymentId);
        Task AddPaymentAsync(PaymentDTO paymentDto);
        Task UpdatePaymentAsync(int paymentId, PaymentDTO paymentDto);
    }
}