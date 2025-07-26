using Entwin.API.Components;
namespace Entwin.API.Models;

public enum Solver
{
    ODE45,
    ODE15,
    EuF1
}

public static class SimulationSettings
{
    public static double Duration { get; set; } = 10.0;
    public static double TimeStep { get; set; } = 0.1;
    public static Solver OdeSolver { get; set; } = Solver.EuF1;
    public static double Time { get; set; } = new();

    public static double stepTime(){
        return Time + TimeStep;
    }
}

public class SimulationRequest
{
    public List<ISimulatable> Components { get; set; } = new();
    public List<Connection> Connections { get; set; } = new();
    public Dictionary<Connection, double> PreviousSignals { get; set; } = new();
}


