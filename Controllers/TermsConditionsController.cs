using G3NexusBackend.DTOs;
using G3NexusBackend.Models;
using G3NexusBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TermsConditionsController : ControllerBase
{
    private readonly ITermsConditionsService _termsConditionsService;

    public TermsConditionsController(ITermsConditionsService termsConditionsService)
    {
        _termsConditionsService = termsConditionsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTermsAsync()
    {
        var TermsConditions = await _termsConditionsService.GetAllTermsAsync();
        return Ok(new ApiResponse { Status = true, Data = TermsConditions });
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTermsById(int id)
    {
        var terms = await _termsConditionsService.GetTermsByIdAsync(id);
        if (terms == null)
        {
            return NotFound(new ApiResponse { Status = false, Message = "Terms not found" });
        }

        return Ok(new ApiResponse { Status = true, Data = terms });
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateTermsAsync(TermsConditionsDTO TermsConditionsdto)
    {
        try
        {
            var terms = await _termsConditionsService.CreateTermsAsync(TermsConditionsdto);
            return CreatedAtAction(nameof(GetTermsById), new { id = terms.TCId }, new ApiResponse { Status = true, Data = terms });
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new ApiResponse { Status = false, Message = ex.Message });
        }
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateTermsAsync(TermsConditionsDTO TermsConditionsdto)
    {
        try
        {
            var TCId = TermsConditionsdto.TCId;
            var term = await _termsConditionsService.UpdateTermsAsync(TCId, TermsConditionsdto);
            if (term == null)
            {
                return NotFound(new ApiResponse { Status = false, Message = "Term not found" });
            }

            return Ok(new ApiResponse { Status = true, Data = term });
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new ApiResponse { Status = false, Message = ex.Message });
        }
    }
    
    [HttpDelete("{TCId:int}")]
    public async Task<IActionResult> DeactivateTerm(int TCId)
    {
        var response = await _termsConditionsService.DeActivateTermAsync(TCId);
        if (!response.Status)
        {
            return NotFound(response);
        }
        return Ok(response);
    }
}