using Microsoft.AspNetCore.Mvc;
using Entwin.API.Models;
using Entwin.API.Data;
using Microsoft.AspNetCore.Authorization;

namespace Entwin.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectSaveController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProjectSaveController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    //[Authorize]
    public async Task<IActionResult> SaveProject([FromBody] Project project)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return Ok(project);
    }
}
