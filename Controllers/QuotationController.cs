using Microsoft.AspNetCore.Mvc;
using G3NexusBackend.Services.Interfaces;
using G3NexusBackend.Data.DTO;
using Microsoft.AspNetCore.Authorization;

namespace G3NexusBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotationController : ControllerBase
    {
        private readonly IQuotationService _quotationService;

        public QuotationController(IQuotationService quotationService)
        {
            _quotationService = quotationService;
        }

        /// <summary>
        /// Get all quotations for admin display
        /// </summary>
        /// <returns>List of all quotations with related data</returns>
    [HttpGet("all")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    public async Task<IActionResult> GetAllQuotations()
        {
            try
            {
                var quotations = await _quotationService.GetAllQuotationsAsync();
                return Ok(new { success = true, data = quotations });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving quotations.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get quotation by ID with full details
        /// </summary>
        /// <param name="id">Quotation ID</param>
        /// <returns>Detailed quotation information</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    public async Task<IActionResult> GetQuotationById(int id)
        {
            try
            {
                var quotation = await _quotationService.GetQuotationByIdAsync(id);
                if (quotation == null)
                {
                    return NotFound(new { success = false, message = "Quotation not found." });
                }
                return Ok(new { success = true, data = quotation });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving the quotation.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get quotations by client ID
        /// </summary>
        /// <param name="clientId">Client ID</param>
        /// <returns>List of quotations for the specified client</returns>
    [HttpGet("client/{clientId}")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    public async Task<IActionResult> GetQuotationsByClientId(int clientId)
        {
            try
            {
                var quotations = await _quotationService.GetQuotationsByClientIdAsync(clientId);
                return Ok(new { success = true, data = quotations });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving quotations for the client.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get quotations by project ID
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <returns>List of quotations for the specified project</returns>
    [HttpGet("project/{projectId}")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    public async Task<IActionResult> GetQuotationsByProjectId(int projectId)
        {
            try
            {
                var quotations = await _quotationService.GetQuotationsByProjectIdAsync(projectId);
                return Ok(new { success = true, data = quotations });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving quotations for the project.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get quotations by employee ID
        /// </summary>
        /// <param name="employeeId">Employee ID</param>
        /// <returns>List of quotations created by the specified employee</returns>
    [HttpGet("employee/{employeeId}")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    public async Task<IActionResult> GetQuotationsByEmployeeId(int employeeId)
        {
            try
            {
                var quotations = await _quotationService.GetQuotationsByEmployeeIdAsync(employeeId);
                return Ok(new { success = true, data = quotations });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving quotations for the employee.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get quotations by type (Bug or Requirement)
        /// </summary>
        /// <param name="type">Quotation type</param>
        /// <returns>List of quotations of the specified type</returns>
    [HttpGet("type/{type}")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    public async Task<IActionResult> GetQuotationsByType(string type)
        {
            try
            {
                var quotations = await _quotationService.GetQuotationsByTypeAsync(type);
                return Ok(new { success = true, data = quotations });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving quotations by type.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get quotations within a date range
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>List of quotations created within the specified date range</returns>
    [HttpGet("date-range")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    public async Task<IActionResult> GetQuotationsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var quotations = await _quotationService.GetQuotationsByDateRangeAsync(startDate, endDate);
                return Ok(new { success = true, data = quotations });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving quotations by date range.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get quotation summary statistics for admin dashboard
        /// </summary>
        /// <returns>Summary statistics of all quotations</returns>
    [HttpGet("summary")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    public async Task<IActionResult> GetQuotationSummary()
        {
            try
            {
                var summary = await _quotationService.GetQuotationSummaryAsync();
                return Ok(new { success = true, data = summary });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving quotation summary.", error = ex.Message });
            }
        }

        /// <summary>
        /// Get detailed items (requirements/bugs) for a specific quotation for modal popup display
        /// </summary>
        /// <param name="quotationId">Quotation ID</param>
        /// <returns>List of requirements and bugs with descriptions for the specified quotation</returns>
    [HttpGet("{quotationId}/items")]
    [Authorize(Roles = "COMPANY_ADMIN")]
    public async Task<IActionResult> GetQuotationItems(int quotationId)
        {
            try
            {
                var items = await _quotationService.GetQuotationItemsAsync(quotationId);
                return Ok(new { success = true, data = items });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving quotation items.", error = ex.Message });
            }
        }
    }
}
