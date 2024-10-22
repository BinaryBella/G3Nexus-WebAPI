using G3NexusBackend.DTOs;

namespace G3NexusBackend.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDTO>> GetAllPaymentsAsync();
        Task<PaymentDTO> GetPaymentByIdAsync(int paymentId);
        Task<PaymentDTO> CreatePaymentAsync(PaymentDTO paymentDto);
        Task<PaymentDTO> UpdatePaymentAsync(int paymentId, PaymentDTO paymentDto);
        Task<ApiResponse> DeActivatePaymentAsync(int paymentId); // Soft delete by setting IsActive to false
    }
}