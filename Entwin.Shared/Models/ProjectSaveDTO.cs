using Entwin.Shared.Components;

namespace Entwin.Shared.Models;

public class ProjectSaveDTO
{
    public int UserId { get; set; }
    public CanvasDTO CanvasData { get; set; }
    public DateTime SavedTime { get; set; }
}

public class CanvasDTO
{
    public List<SimulatableDTOBase> Components { get; set; }
    public List<ConnectionDTO> Connections { get; set; }
}