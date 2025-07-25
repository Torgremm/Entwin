using Entwin.API.Models;
using Entwin.API.Controllers;

namespace Entwin.API.Components;

public class TransferFunctionComponent : TimeSimulation, ISimulatable
{
    public int Id { get; set; } = new();
    public List<double> Numerator { get; set; } = new();
    public List<double> Denominator { get; set; } = new();

    private List<double> _inputHistory = new();
    private List<double> _outputHistory = new();

    private double _currentTime = 0.0;
    public double SimulateStep(double input, double currentTime)
    {
        _currentTime = currentTime;

        var tfRequest = new TransferFunctionRequest
        {
            Numerator = this.Numerator,
            Denominator = this.Denominator,
            TimeStep = this.TimeStep,
            Input = input,
            Time = _currentTime,
            InputHistory = _inputHistory,
            OutputHistory = _outputHistory
        };

        var response = SimulationController.SimulateTransferFunction(tfRequest);

        _inputHistory = response.InputHistory;
        _outputHistory = response.OutputHistory;

        return response.Output;
    }
}