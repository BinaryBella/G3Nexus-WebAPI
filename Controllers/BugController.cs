using G3NexusBackend.DTOs;
using G3NexusBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetBugs()
        {
            var bugs = await _bugService.GetAllBugsAsync();
            return Ok(new ApiResponse { Status = true, Data = bugs });
        }

        [HttpGet("{id:int}")]
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

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeactivateBug(int id)
        {
            var response = await _bugService.DeActivateBugAsync(id);
            if (!response.Status)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}

