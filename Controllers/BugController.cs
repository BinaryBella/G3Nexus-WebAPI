using G3NexusBackend.Data.DTO;
using G3NexusBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace G3NexusBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BugController : ControllerBase
    {
        private readonly IBugService _bugService;

        public BugController(IBugService bugService)
        {
            _bugService = bugService;
        }

        [HttpGet]
        [Authorize(Roles = "CLIENT_ADMIN,CLIENT_USER,COMPANY_ADMIN,COMPANY_DEVELOPER")]
        public async Task<IActionResult> GetBugs([FromQuery] int userId, [FromQuery] DateTime lastLogin)
        {
            var bugs = await _bugService.GetAllBugsAsync(userId);
            return Ok(new ApiResponse { Status = true, Data = bugs });
        }
        
        [HttpGet("{id:int}")]
        [Authorize(Roles = "CLIENT_ADMIN,CLIENT_USER,COMPANY_ADMIN,COMPANY_DEVELOPER")]
        public async Task<IActionResult> GetBugById(int id)
        {
            var bug = await _bugService.GetBugByIdAsync(id);
            if (bug == null)
            {
                return NotFound(new ApiResponse { Status = false, Message = "Bug not found" });
            }
            return Ok(new ApiResponse { Status = true, Data = bug });
        }

        [HttpPost]
        [Authorize(Roles = "CLIENT_ADMIN,CLIENT_USER")]
        public async Task<IActionResult> CreateBug(BugDTO bugDto)
        {
            try
            {
                var bug = await _bugService.CreateBugAsync(bugDto);
                return CreatedAtAction(nameof(GetBugById), new { id = bug.BugId }, new ApiResponse { Status = true, Data = bug });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new ApiResponse { Status = false, Message = ex.Message });
            }
        }

        [HttpPut]
        [Authorize(Roles = "COMPANY_ADMIN,COMPANY_DEVELOPER")]
        public async Task<IActionResult> UpdateBug(BugDTO bugDto)
        {
            try
            {
                var bugId = bugDto.BugId;
                var bug = await _bugService.UpdateBugAsync(bugId, bugDto);
                if (bug == null)
                {
                    return NotFound(new ApiResponse { Status = false, Message = "Bug not found" });
                }
                return Ok(new ApiResponse { Status = true, Data = bug });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new ApiResponse { Status = false, Message = ex.Message });
            }
        }

    [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "COMPANY_ADMIN,COMPANY_DEVELOPER")]
        public async Task<IActionResult> UpdateBugStatus(int id, [FromBody] StatusUpdateDTO statusUpdate)
        {
            var response = await _bugService.UpdateBugStatusAsync(id, statusUpdate.Status);
            if (!response.Status)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "COMPANY_ADMIN,COMPANY_DEVELOPER")]
        public async Task<IActionResult> DeactivateBug(int id)
        {
            var response = await _bugService.DeActivateBugAsync(id);
            if (!response.Status)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost("send-quotation")]
        [Authorize(Roles = "COMPANY_ADMIN")]
        public async Task<IActionResult> SendBugQuotation(BugQuotationRequestDTO quotationRequest)
        {
            try
            {
                var response = await _bugService.SendBugQuotationAsync(quotationRequest);
                if (!response.Status)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse 
                { 
                    Status = false, 
                    Message = $"Internal server error: {ex.Message}" 
                });
            }
        }

        [HttpPost("send-bulk-quotation")]
        [Authorize(Roles = "COMPANY_ADMIN")]
        public async Task<IActionResult> SendBulkBugQuotation(BulkBugQuotationRequestDTO bulkQuotationRequest)
        {
            try
            {
                var response = await _bugService.SendBulkBugQuotationAsync(bulkQuotationRequest);
                if (!response.Status)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse 
                { 
                    Status = false, 
                    Message = $"Internal server error: {ex.Message}" 
                });
            }
        }
    }
}

