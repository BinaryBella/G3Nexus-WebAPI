using G3NexusBackend.DTOs;

namespace G3NexusBackend.Interfaces
{
    public interface IPaymentService
    {
        Task<ApiResponse> AddPayment(PaymentDTO paymentDto);
        Task<ApiResponse> EditPayment(PaymentDTO paymentDto);
        Task<ApiResponse> GetAllPayments();
        Task<ApiResponse> GetPaymentById(int paymentId);
    }
}