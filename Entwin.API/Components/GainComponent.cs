using Entwin.API.Models;

namespace Entwin.API.Components;

public class GainComponent : ISimulatable
{
    public double value { get; set; } = new();
    public int Id { get; set; } = new();

    public double SimulateStep(double input, double currentTime)
    {
        return input * value;
    }
}