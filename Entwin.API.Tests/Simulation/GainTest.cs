using Xunit;
using Entwin.API.Models;
using Entwin.API.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Entwin.API.Controllers;
using ScottPlot;

namespace Entwin.API.Tests.Simulation;

public class GainTests
{
    [Fact]
    public void GainResponse_ShouldConstantGain(){
        var stepComponent = new StepComponent(1,2,4,1);
        var gainComponent = new GainComponent(2.34, 2);

        var inputConnection = new Connection(stepComponent.Id, gainComponent.Id);
        var outputConnection = new Connection(gainComponent.Id, -1);

        var request = new SimulationRequest
        {
            Components = new List<ISimulatable> { gainComponent, stepComponent },
            Connections = new List<Connection> {outputConnection, inputConnection},
            PreviousSignals = new Dictionary<Connection, double>()
        };

        double duration = 10.0;
        int steps = (int)(duration / SimulationSettings.TimeStep);
        var outputs = new List<double>();
        double currentTime = 0.0;

        for (int i = 0; i < steps; i++)
        {
            var response = Services.CanvasSimulation.SimulateCanvas(request);

            request.PreviousSignals = response.PreviousSignals;
            currentTime = response.Time;
            double output = response.ConnectionSignals[outputConnection];
            outputs.Add(output);
        }

        Assert.True(outputs.Last() == 4.68, $"Final output should be double the gain - {outputs.Last()}");
        Assert.True(outputs[2] == 2.34, $"Initial output should be equal to gain - {outputs.First()}");
    }

}
