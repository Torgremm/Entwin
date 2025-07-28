using Xunit;
using Entwin.API.Models;
using Entwin.API.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Entwin.API.Controllers;
using ScottPlot;

namespace Entwin.API.Tests.Simulation;
[Trait("Category","Simulation")]
public class StepTests
{
    [Fact]
    public void StepSignal_ShouldMatchExpectedValues_At5And10Seconds(){
        // Arrange
        var stepComponent = new StepComponent(0.3, 1.0, 5.0, 100);

        var Numerator = new List<double> { 1.0 };
        var Denominator = new List<double> { 1.0, 1.0 };

        var connection = new Connection(100,200);

        var request = new SimulationRequest
        {
            Components = new List<ISimulatable> { stepComponent },
            Connections = new List<Connection> { connection },
            PreviousSignals = new Dictionary<Connection, double>()
        };

        var transferFunction = new TransferFunctionComponent(Numerator, Denominator, 200, request);
        request.Components.Add(transferFunction);

        double t = 0.0;
        double outputAt5 = 0.0;
        double outputAt10 = 0.0;

        while (t < 10.0){
            var response = Services.CanvasSimulation.SimulateCanvas(request);

            if (Math.Abs(response.Time - 5.0) < 0.1)
                outputAt5 = response.ConnectionSignals[connection];

            if (Math.Abs(response.Time - 10.0) < 0.1)
                outputAt10 = response.ConnectionSignals[connection];

            // Prepare request for next step
            request.PreviousSignals = response.PreviousSignals;
            request.settings.Time = response.Time;
            t = response.Time;
        }

        Assert.True(outputAt5 - 0.3 < 0.01, $"Output should match at five second mark - {outputAt5}");
        Assert.True(outputAt10 - 1.0 < 0.01, $"Output should match at ten second mark - {outputAt10}");

    }

}
