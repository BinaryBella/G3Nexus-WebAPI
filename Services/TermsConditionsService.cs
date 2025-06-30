using G3NexusBackend.DTOs;
using G3NexusBackend.Models;
using G3NexusBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace G3NexusBackend.Services;

public class TermsConditionsService : ITermsConditionsService
{
    private readonly G3NexusDbContext _context;

    public TermsConditionsService(G3NexusDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TermsConditionsDTO>> GetAllTermsAsync()
    {
        return await _context.TermsConditions
            .Select(t => new TermsConditionsDTO
            {
                TCId = t.TCId,
                Content = t.Content,
                UpdatedDate = t.UpdatedDate,
                IsActive = t.IsActive
            })
            .ToListAsync();
    }
    
    public async Task<TermsConditionsDTO?> GetTermsByIdAsync(int TCId)
    {
        var term = await _context.TermsConditions.FindAsync(TCId);

        return new TermsConditionsDTO
        {
            TCId = term.TCId,
            Content = term.Content,
            UpdatedDate = term.UpdatedDate,
            IsActive = term.IsActive
        };
    }

    
    public async Task<TermsConditionsDTO> CreateTermsAsync(TermsConditionsDTO TermsConditionsdto)
    {
        var term = new TermsConditions
        {
            TCId = TermsConditionsdto.TCId,
            Content = TermsConditionsdto.Content,
            UpdatedDate = TermsConditionsdto.UpdatedDate,
            IsActive = TermsConditionsdto.IsActive
        };

        _context.TermsConditions.Add(term);
        await _context.SaveChangesAsync();

        TermsConditionsdto.TCId = term.TCId;
        return TermsConditionsdto;
    }
    
    public async Task<TermsConditionsDTO?> UpdateTermsAsync(int TCId, TermsConditionsDTO TermsConditionsdto)
    {
        var term = await _context.TermsConditions.FindAsync(TCId);

        term.TCId = TermsConditionsdto.TCId;
        term.Content = TermsConditionsdto.Content;
        term.UpdatedDate = TermsConditionsdto.UpdatedDate;
        term.IsActive = TermsConditionsdto.IsActive;
        
        _context.TermsConditions.Update(term);
        await _context.SaveChangesAsync();

        return TermsConditionsdto;
    }
    
    public async Task<ApiResponse> DeActivateTermAsync(int id)
    {
        var term = await _context.TermsConditions.FindAsync(id);
        if (term is not {IsActive: true})
        {
            return new ApiResponse { Status = false, Message = "Term not found or already inactive." };
        }

        term.IsActive = false;
        _context.TermsConditions.Update(term);
        await _context.SaveChangesAsync();

        return new ApiResponse { Status = true, Message = "Term successfully deactivated." };
    }
}
