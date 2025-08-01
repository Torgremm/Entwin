using Entwin.Shared.Models;

namespace Entwin.Client.Services;

public class SimulationState
{
    public SimulationResultDTO? LastResult { get; set; }

    public double Duration { get; set; } = 10;
    public double TimeStep { get; set; } = 0.01;
    
}
