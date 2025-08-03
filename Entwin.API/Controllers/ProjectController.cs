using Microsoft.AspNetCore.Mvc;
using Entwin.API.Models;
using Entwin.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Entwin.Shared.Models;
using System.Text.Json;

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
    public async Task<IActionResult> SaveProject([FromBody] ProjectSaveDTO projectDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized("User ID not found.");

        var existingProject = await _context.Projects
            .FirstOrDefaultAsync(p => p.Name == projectDto.Name && p.UserId == userId);

        if (existingProject != null)
        {
            var updatedModel = ComponentMapper.MapToModel(projectDto);
            existingProject.CanvasData = updatedModel.CanvasData;
            existingProject.SavedTime = updatedModel.SavedTime;
        }
        else
        {
            var projectModel = ComponentMapper.MapToModel(projectDto);
            projectModel.UserId = userId;
            _context.Projects.Add(projectModel);
        }

        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("load")]
    public async Task<IActionResult> LoadProject([FromBody] string projectName)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized("User ID not found.");

        var prj = await _context.Projects
            .FirstOrDefaultAsync(p => p.Name == projectName && p.UserId == userId);

        if (prj == null)
            return NotFound();

        var dto = ComponentMapper.MapToDTO(prj);
        return Ok(dto);
    }

    [HttpGet("load")]
    public async Task<IActionResult> GetProjects()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized("User ID not found.");

        var projects = await _context.Projects
            .Where(p => p.UserId == userId)
            .ToListAsync();

        var dtos = projects.Select(ComponentMapper.MapToDTO).ToList();
        return Ok(dtos);
    }
}
