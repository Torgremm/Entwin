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
    public double SimulateStep(double[] input, double currentTime)
    {
        return currentTime > SwitchTime ? EndValue : StartValue;
    }

    public double[] SortedInput(double[] input, int[] Ids)
    {
        return input;
    }
}