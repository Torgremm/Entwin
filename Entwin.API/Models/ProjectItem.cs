using System.ComponentModel.DataAnnotations.Schema;
using Entwin.API.Components;

namespace Entwin.API.Models;
public class Project
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool ProjectData { get; set; } //change this to the project save
    public DateTime SavedTime { get; set; }
}

