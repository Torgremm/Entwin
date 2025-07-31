using Entwin.Shared.Components;
namespace Entwin.API.Components;

public class GainComponent : ISimulatable
{
    public double Value { get; set; } = new();
    public int Id { get; set; } = new();

    public GainComponent(double v, int id)
    {
        Value = v;
        Id = id;
    }
    public GainComponent(GainDTO gain)
    {
        Id = gain.Id;
        Value = gain.Value;
    }
    public double[] SimulateStep(double[] input, double currentTime)
    {
        return [input[0] * Value];
    }
}