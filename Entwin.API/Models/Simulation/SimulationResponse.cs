using Entwin.API.Components;
namespace Entwin.API.Models;
public class SimulationResponse
{
    public double Time { get; set; }
    public Dictionary<Connection, double> ConnectionSignals { get; set; } = new();
    public Dictionary<Connection, double> PreviousSignals { get; set; } = new();
}
