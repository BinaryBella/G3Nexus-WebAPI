using G3NexusBackend.Data.DTO;

namespace G3NexusBackend.Services.Interfaces;

public interface IPaymentService
{
    Task<IEnumerable<PaymentDTO>> GetAllPaymentsAsync();
    Task<PaymentDTO?> GetPaymentByIdAsync(int paymentId);
    Task<IEnumerable<PaymentDTO>> GetPaymentsByClientIdAsync(int clientId);
    Task<PaymentDTO> CreatePaymentAsync(PaymentDTO paymentDto);
    Task<PaymentDTO?> UpdatePaymentAsync(int paymentId, PaymentDTO paymentDto);
    Task<ApiResponse> DeActivatePaymentAsync(int paymentId); // Soft delete by setting IsActive to false
}