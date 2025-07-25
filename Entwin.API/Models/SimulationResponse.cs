using Entwin.API.Components;
namespace Entwin.API.Models;
public class SimulationResponse
{
    public double Time { get; set; }
    // Key: "FromId-ToId"
    // Value: signal timeseries flowing on that connection
    public Dictionary<Connection, double> ConnectionSignals { get; set; } = new();
    public Dictionary<Connection, double> PreviousSignals { get; set; } = new();
}
