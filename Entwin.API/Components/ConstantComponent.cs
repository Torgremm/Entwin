namespace Entwin.API.Components;

public class ConstantComponent : ISimulatable
{
    public double value { get; set; } = new();
    public int Id { get; set; } = new();

    public ConstantComponent(double v, int id)
    {
        value = v;
        Id = id;
    }
    public double SimulateStep(double[] input, double currentTime)
    {
        return value;
    }

}