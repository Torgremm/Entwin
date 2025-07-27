namespace Entwin.API.Components;

public interface ISimulatable
{
    public int Id { get; set; }
    double SimulateStep(double[] input, double time);

    double[] SortedInput(double[] input, int[] Ids);
}
