using G3NexusBackend.DTOs;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using G3NexusBackend.Interfaces;
using G3NexusBackend.Models;

public class TermsConditionsService : ITermsConditionsService
{
    private readonly List<TermsConditions> _termsConditions;

    public TermsConditionsService()
    {
        _termsConditions = new List<TermsConditions>();
    }

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
        var newTC = new TermsConditions
        {
            TCId = _termsConditions.Count + 1,
            Content = dto.Content,
            UpdatedDate = DateTime.Now
        };

        _termsConditions.Add(newTC);

        return new ApiResponse
        {
            Status = true,
            Message = "Terms and Conditions created successfully",
            Data = newTC
        };
    }

    public async Task<ApiResponse> UpdateAsync(TermsConditionsDTO dto)
    {
        var existingTC = _termsConditions.FirstOrDefault(tc => tc.TCId == dto.TCId);

        if (existingTC == null)
        {
            return new ApiResponse
            {
                Status = false,
                Message = "Terms and Conditions not found",
                Error = "Invalid TCId"
            };
        }

        existingTC.Content = dto.Content;
        existingTC.UpdatedDate = DateTime.Now;

        return new ApiResponse
        {
            Status = true,
            Message = "Terms and Conditions updated successfully",
            Data = existingTC
        };
    }
}