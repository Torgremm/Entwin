using System.ComponentModel.DataAnnotations.Schema;
using Entwin.API.Components;
using Entwin.Shared.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;


namespace Entwin.API.Models;

public class ProjectModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    [ForeignKey("User")]
    public string UserId { get; set; } = string.Empty;
    public IdentityUser? User { get; set; }
    public string CanvasDataJson { get; set; } = string.Empty;
    public DateTime SavedTime { get; set; }

    [NotMapped]
    public CanvasDTO CanvasData
    {
        get => string.IsNullOrEmpty(CanvasDataJson)
            ? new CanvasDTO()
            : JsonSerializer.Deserialize<CanvasDTO>(CanvasDataJson) ?? new CanvasDTO();
        set => CanvasDataJson = JsonSerializer.Serialize(value);
    }
    
    public ProjectSaveDTO MapToDTO(ProjectModel model)
    {
        return new ProjectSaveDTO
        {
            Name = "My Project",
            CanvasData = model.CanvasData,
            SavedTime = model.SavedTime
        };
    }

    public ProjectModel MapToModel(ProjectSaveDTO dto, string userId)
    {
        return new ProjectModel
        {
            UserId = userId,
            SavedTime = dto.SavedTime,
            CanvasData = dto.CanvasData
        };
    }
}


