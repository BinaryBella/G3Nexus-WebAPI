using G3NexusBackend.DTOs;
using Microsoft.AspNetCore.Mvc;
using G3NexusBackend.Interfaces;

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
    public async Task<IActionResult> GetAll()
    {
        var response = await _termsConditionsService.GetAllAsync();
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TermsConditionsDTO dto)
    {
        var response = await _termsConditionsService.CreateAsync(dto);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> Update(TermsConditionsDTO dto)
    {
        var response = await _termsConditionsService.UpdateAsync(dto);
        return Ok(response);
    }
}