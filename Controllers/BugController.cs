using G3NexusBackend.DTOs;
using G3NexusBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace G3NexusBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BugController : ControllerBase
    {
        private readonly IBugService _bugService;

        public BugController(IBugService bugService)
        {
            _bugService = bugService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddBug([FromBody] BugDTO bugDto)
        {
            var response = await _bugService.AddBug(bugDto);
            return Ok(response);
        }

        [HttpPut("edit")]
        public async Task<IActionResult> EditBug([FromBody] BugDTO bugDto)
        {
            var response = await _bugService.EditBug(bugDto);
            return Ok(response);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllBugs()
        {
            var response = await _bugService.GetAllBugs();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBugById(int id)
        {
            var response = await _bugService.GetBugById(id);
            return Ok(response);
        }
    }

}