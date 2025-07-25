namespace Entwin.API.Models;

public class TransferFunctionResponse
{
    public double Time { get; set; } = new();
    public double Output { get; set; } = new();
    public List<double> OutputHistory { get; set; } = new();
    public List<double> InputHistory { get; set; } = new();
}
