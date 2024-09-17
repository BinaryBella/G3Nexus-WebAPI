using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace G3NexusBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BugsController : ControllerBase
    {
        private readonly IBugService _bugService;

        public BugsController(IBugService bugService)
        {
            _bugService = bugService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BugDTO>>> GetAllBugs()
        {
            var bugs = await _bugService.GetAllBugsAsync();
            return Ok(bugs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BugDTO>> GetBugById(int id)
        {
            var bug = await _bugService.GetBugByIdAsync(id);
            if (bug == null) return NotFound();

            return Ok(bug);
        }

        [HttpPost]
        public async Task<ActionResult> AddBug(BugDTO bugDto)
        {
            await _bugService.AddBugAsync(bugDto);
            return CreatedAtAction(nameof(GetBugById), new { id = bugDto.BugId }, bugDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBug(int id, BugDTO bugDto)
        {
            await _bugService.UpdateBugAsync(id, bugDto);
            return NoContent();
        }
    }
}