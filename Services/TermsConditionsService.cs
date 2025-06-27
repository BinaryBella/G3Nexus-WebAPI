using G3NexusBackend.DTOs;
using G3NexusBackend.Models;
using G3NexusBackend.Services.Interfaces;

namespace G3NexusBackend.Services;

public class TermsConditionsService : ITermsConditionsService
{
    private readonly List<TermsConditions> _termsConditions = new();

    public async Task<ApiResponse> GetAllAsync()
    {
        return new ApiResponse
        {
            Status = true,
            Message = "Fetched all terms and conditions successfully",
            Data = _termsConditions
        };
    }

    public async Task<ApiResponse> CreateAsync(TermsConditionsDTO dto)
    {
        var newTc = new TermsConditions
        {
            TCId = _termsConditions.Count + 1,
            Content = dto.Content,
            UpdatedDate = DateTime.Now
        };

        _termsConditions.Add(newTc);

        return new ApiResponse
        {
            Status = true,
            Message = "Terms and Conditions created successfully",
            Data = newTc
        };
    }

    public async Task<ApiResponse> UpdateAsync(TermsConditionsDTO dto)
    {
        var existingTc = _termsConditions.FirstOrDefault(tc => tc.TCId == dto.TCId);

        if (existingTc == null)
        {
            return new ApiResponse
            {
                Status = false,
                Message = "Terms and Conditions not found",
                Error = "Invalid TCId"
            };
        }

        existingTc.Content = dto.Content;
        existingTc.UpdatedDate = DateTime.Now;

        return new ApiResponse
        {
            Status = true,
            Message = "Terms and Conditions updated successfully",
            Data = existingTc
        };
    }
}