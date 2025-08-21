using G3NexusBackend.Data.DTO;
using G3NexusBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace G3NexusBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceChartsController : ControllerBase
    {
        private readonly IPerformanceChartsService _performanceChartsService;

        public PerformanceChartsController(IPerformanceChartsService performanceChartsService)
        {
            _performanceChartsService = performanceChartsService ?? throw new ArgumentNullException(nameof(performanceChartsService));
        }

        /// <summary>
        /// Get all performance chart data in one response
        /// </summary>
        /// <returns>Combined performance charts data</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPerformanceData()
        {
            try
            {
                var data = await _performanceChartsService.GetAllPerformanceDataAsync();
                return Ok(new ApiResponse 
                { 
                    Status = true, 
                    Data = data,
                    Message = "Performance data retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Status = false,
                    Message = $"Error retrieving performance data: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get project overview data for the specified number of months
        /// </summary>
        /// <param name="monthsBack">Number of months to look back (default: 6)</param>
        /// <returns>Project overview chart data</returns>
        [HttpGet("projects")]
        public async Task<IActionResult> GetProjectOverviewData([FromQuery] int monthsBack = 6)
        {
            try
            {
                if (monthsBack <= 0 || monthsBack > 24)
                {
                    return BadRequest(new ApiResponse
                    {
                        Status = false,
                        Message = "Months back must be between 1 and 24"
                    });
                }

                var data = await _performanceChartsService.GetProjectOverviewDataAsync(monthsBack);
                return Ok(new ApiResponse
                {
                    Status = true,
                    Data = data,
                    Message = "Project overview data retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Status = false,
                    Message = $"Error retrieving project overview data: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get requirements status distribution data
        /// </summary>
        /// <returns>Requirements status chart data</returns>
        [HttpGet("requirements")]
        public async Task<IActionResult> GetRequirementsStatusData()
        {
            try
            {
                var data = await _performanceChartsService.GetRequirementsStatusDataAsync();
                return Ok(new ApiResponse
                {
                    Status = true,
                    Data = data,
                    Message = "Requirements status data retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Status = false,
                    Message = $"Error retrieving requirements status data: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get bug trend data for the specified number of weeks
        /// </summary>
        /// <param name="weeksBack">Number of weeks to look back (default: 6)</param>
        /// <returns>Bug trend chart data</returns>
        [HttpGet("bugs")]
        public async Task<IActionResult> GetBugTrendData([FromQuery] int weeksBack = 6)
        {
            try
            {
                if (weeksBack <= 0 || weeksBack > 52)
                {
                    return BadRequest(new ApiResponse
                    {
                        Status = false,
                        Message = "Weeks back must be between 1 and 52"
                    });
                }

                var data = await _performanceChartsService.GetBugTrendDataAsync(weeksBack);
                return Ok(new ApiResponse
                {
                    Status = true,
                    Data = data,
                    Message = "Bug trend data retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Status = false,
                    Message = $"Error retrieving bug trend data: {ex.Message}"
                });
            }
        }
    }
}
