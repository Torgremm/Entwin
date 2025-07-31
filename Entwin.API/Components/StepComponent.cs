using Entwin.Shared.Components;

namespace Entwin.API.Components;

public class StepComponent : ISimulatable
{
    public double StartValue { get; set; } = new();
    public double EndValue { get; set; } = new();
    public double SwitchTime { get; set; } = new();
    public int Id { get; set; } = new();

    public StepComponent(double start, double end, double s, int id)
    {
        StartValue = start;
        EndValue = end;
        SwitchTime = s;
        Id = id;
    }
    public StepComponent(StepDTO step)
    {
        Id = step.Id;
        StartValue = step.StartValue;
        EndValue = step.EndValue;
        SwitchTime = step.SwitchTime;
    }
    public double[] SimulateStep(double[] input, double currentTime)
    {
        return [currentTime > SwitchTime ? EndValue : StartValue];
    }
}