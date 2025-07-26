using Xunit;
using Entwin.API.Models;
using Entwin.API.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Entwin.API.Controllers;
using ScottPlot;

namespace Entwin.API.Tests;

public class StepTests
{
    [Fact]
    public void StepSignal_ShouldMatchExpectedValues_At5And10Seconds(){
        // Arrange
        var stepComponent = new StepComponent(0.3, 1.0, 5.0, 100);

        var Numerator = new List<double> { 1.0 };
        var Denominator = new List<double> { 1.0, 1.0 };
        var transferFunction = new TransferFunctionComponent(Numerator, Denominator, 200);

        var connection = new Connection(100,200);

        var request = new SimulationRequest
        {
            Components = new List<ISimulatable> { stepComponent, transferFunction },
            Connections = new List<Connection> { connection },
            PreviousSignals = new Dictionary<Connection, double>()
        };

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
            SimulationSettings.Time = response.Time;
            t = response.Time;
        }

        Assert.Equal(0.3, outputAt5, 3);
        Assert.Equal(1.0, outputAt10, 3);
    }

}
