namespace Entwin.Shared.Components;

public class TransferFunctionComponentDTO : ISimulatableDTO
{
    public int Id { get; set; }
    public List<double> sNumerator { get; set; } = new();
    public List<double> sDenominator { get; set; } = new();
}