using G3NexusBackend.Data.DTO;
using G3NexusBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace G3NexusBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService service)
    {
        _companyService = service;
    }

    [HttpGet]
    [Authorize(Roles = "COMPANY_ADMIN,COMPANY_DEVELOPER")]
    public async Task<IActionResult> GetAllCompaniesAsync()
    {
        var companies = await _companyService.GetAllCompaniesAsync();
        return Ok(new ApiResponse { Status = true, Data = companies });
    }

    [HttpGet("{CompanyId:int}")]
    [Authorize(Roles = "COMPANY_ADMIN,COMPANY_DEVELOPER")]
    public async Task<IActionResult> GetCompanyById(int CompanyId)
    {
        var company = await _companyService.GetCompanyByIdAsync(CompanyId);
        if (company == null)
        {
            return NotFound(new ApiResponse { Status = false, Message = "Company not found" });
        }

        return Ok(new ApiResponse { Status = true, Data = company });
    }

    [HttpPost]
    [Authorize(Roles = "COMPANY_ADMIN")]
    public async Task<IActionResult> CreateCompaniesAsync(CompanyDTO companyDto)
    {
        var response = await _companyService.CreateCompaniesAsync(companyDto);
        if (!response.Status)
        {
            return BadRequest(response);
        }

        return CreatedAtAction(nameof(GetCompanyById), new { CompanyId = companyDto.CompanyId }, response);
    }

    [HttpPut]
    [Authorize(Roles = "COMPANY_ADMIN")]
    public async Task<IActionResult> UpdateCompaniesAsync(CompanyDTO companyDto)
    {
        var company = await _companyService.UpdateCompaniesAsync(companyDto);
        if (company == null)
        {
            return NotFound(new ApiResponse { Status = false, Message = "Company not found" });
        }

        return Ok(new ApiResponse { Status = true, Data = company });
    }

    [HttpDelete("{CompanyId:int}")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    public async Task<IActionResult> DeActivateCompanyAsync(int CompanyId)
    {
        var response = await _companyService.DeActivateCompanyAsync(CompanyId);
        if (!response.Status)
        {
            return NotFound(response);
        }

        return Ok(response);
    }
}