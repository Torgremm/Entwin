using Entwin.Shared.Components;

namespace Entwin.API.Components;

public class ConstantComponent : ISimulatable
{
    public double Value { get; set; } = new();
    public int Id { get; set; } = new();

    public ConstantComponent(double v, int id)
    {
        Value = v;
        Id = id;
    }
    public ConstantComponent(ConstantDTO constant)
    {
        Id = constant.Id;
        Value = constant.Value;
    }
    public double[] SimulateStep(double[] input, double currentTime)
    {
        return [Value];
    }

}