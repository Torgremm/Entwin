using Entwin.Shared.Components;

namespace Entwin.API.Components;

public class SumComponent : ISimulatable
{
    public int Id { get; set; }
    public List<bool> Sign { get; set; }

    public SumComponent(List<bool> signs, int id)
    {
        Id = id;
        Sign = signs;
    }

    public SumComponent(SumDTO sum)
    {
        List<bool> signs = new();
        Id = sum.Id;

        if (sum.Signs == null)
            throw new Exception("Misconfigured summation, must be a string of '+' and '-");
            
        foreach (char c in sum.Signs)
        {
            signs.Add(c == '+' ? true : false);
        }
        Sign = signs;
    }

    public double[] SimulateStep(double[] input, double currentTime)
    {
        double sum = 0;
        for (int ii = 0; ii < input.Length; ii++)
        {
            sum = Sign[ii] ? sum + input[ii] : sum - input[ii];
        }
        return [sum];
    }
}
