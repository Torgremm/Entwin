using Microsoft.AspNetCore.Mvc;
using Entwin.API.Models;
using Entwin.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Entwin.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProjectController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveProject([FromBody] ProjectModel project)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized("User ID not found.");

        project.UserId = userId;

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return Ok(project);
    }

    [HttpPost("load")]
    public async Task<IActionResult> LoadProject([FromBody] ProjectModel project)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    
        if (userId == null)
            return Unauthorized("User ID not found.");
    
        var prj = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == project.Id && p.UserId == userId);
    
        if (prj == null)
            return NotFound();
    
        return Ok(prj);
    }

    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized("User ID not found.");

        var projects = await _context.Projects
            .Where(p => p.UserId == userId)
            .ToListAsync();

        return Ok(projects);
    }
}
