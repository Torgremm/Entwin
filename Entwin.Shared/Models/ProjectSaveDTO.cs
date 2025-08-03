using Entwin.Shared.Components;

namespace Entwin.Shared.Models;

public class ProjectSaveDTO
{
    public required CanvasDTO CanvasData { get; set; }
    public DateTime SavedTime { get; set; }
}

public class CanvasDTO
{
    public List<ComponentSaveDTO>? Components { get; set; }
    public List<ConnectionDTO>? Connections { get; set; }
}