using Entwin.Shared.Components;

namespace Entwin.Shared.Models;

public enum Solver
{
    ODE45,
    ODE15,
    EuF1
}

public class SimulationSettingsDTO
{
    public double Duration { get; set; } = 10;
    public double TimeStep { get; set; } = 0.1;
    public Solver OdeSolver { get; set; } = Solver.EuF1;
    public double Time { get; set; } = 0.0;
}


public class SimulationRequestDTO
{
    public SimulationSettingsDTO Settings { get; set; } = new();
    public List<ISimulatableDTO> Components { get; set; } = new();
    public List<ConnectionDTO> Connections { get; set; } = new();
    public Dictionary<ConnectionDTO, double> PreviousSignals { get; set; } = new();
}


