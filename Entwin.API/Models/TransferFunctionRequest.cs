namespace Entwin.API.Models;

public class TransferFunctionRequest : TimeSimulation  {
    public List<double> Numerator {get;set;} = new();
    public List<double> Denominator {get;set;} = new();

    public List<double> InputHistory { get; set; } = new();
    public List<double> OutputHistory { get; set; } = new();

    public double Input { get; set; } = 1.0;    
}
