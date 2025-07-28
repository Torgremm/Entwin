using Entwin.API.Components;
namespace Entwin.API.Models;

public enum Solver
{
    ODE45,
    ODE15,
    EuF1
}

public class SimulationSettings
{
    public double Duration { get; set; } = 10.0;
    public double TimeStep { get; set; } = 0.1;
    public Solver OdeSolver { get; set; } = Solver.EuF1;
    public double Time { get; set; } = 0.0;

    public double StepTime()
    {
        return Time + TimeStep;
    }
}


public class SimulationRequest
{
    public SimulationSettings settings { get; set; } = new();
    public List<ISimulatable> Components { get; set; } = new();
    public List<Connection> Connections { get; set; } = new();
    public Dictionary<Connection, double> PreviousSignals { get; set; } = new();
}


