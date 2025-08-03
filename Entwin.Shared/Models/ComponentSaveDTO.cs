using Entwin.Shared.Components;

namespace Entwin.Shared.Models;

public class ComponentSaveDTO
{
    public required SimulatableDTOBase SimulationData { get; set; }
    public required UIData VisualData{ get; set; }
}

public class UIData
{
    public double X { get; set; }
    public double Y { get; set; }
    public int InputCount{ get; set; }
    public int OutputCount{ get; set; }
}