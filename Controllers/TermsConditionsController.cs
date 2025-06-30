using G3NexusBackend.Data.DTO;
using G3NexusBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace G3NexusBackend.Controllers;

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
        var termsConditions = await _termsConditionsService.GetAllTermsAsync();
        return Ok(new ApiResponse { Status = true, Data = termsConditions });
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
    public async Task<IActionResult> CreateTermsAsync(TermsConditionsDTO termsConditionsDto)
    {
        try
        {
            var terms = await _termsConditionsService.CreateTermsAsync(termsConditionsDto);
            return CreatedAtAction(nameof(GetTermsById), new { id = terms.TCId }, new ApiResponse { Status = true, Data = terms });
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new ApiResponse { Status = false, Message = ex.Message });
        }
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateTermsAsync(TermsConditionsDTO termsConditionsDto)
    {
        try
        {
            var tcId = termsConditionsDto.TCId;
            var term = await _termsConditionsService.UpdateTermsAsync(tcId, termsConditionsDto);
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
    
    [HttpDelete("{tcId:int}")]
    public async Task<IActionResult> DeactivateTerm(int tcId)
    {
        var response = await _termsConditionsService.DeActivateTermAsync(tcId);
        if (!response.Status)
        {
            return NotFound(response);
        }
        return Ok(response);
    }
}