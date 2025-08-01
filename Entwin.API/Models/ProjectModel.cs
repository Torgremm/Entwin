using System.ComponentModel.DataAnnotations.Schema;
using Entwin.API.Components;
using Microsoft.AspNetCore.Identity;

namespace Entwin.API.Models;
public class ProjectModel
{
    public int Id { get; set; }

    [ForeignKey("User")]
    public string UserId { get; set; } = string.Empty;
    public IdentityUser? User { get; set; }
    public bool ProjectData { get; set; } //change this to the project save
    public DateTime SavedTime { get; set; }
}

