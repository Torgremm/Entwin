using Xunit;
using Entwin.API.Models;
using Entwin.API.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Entwin.API.Controllers;
using ScottPlot;

namespace Entwin.API.Tests.Simulation;
[Collection("SimulationTests")]
public class ConstantTests
{
    [Fact]
    public void UnitStepResponse_ShouldApproximateFirstOrderRise(){
        var tfComponent = new TransferFunctionComponent(
            new List<double> { 1 }, 
            new List<double> { 1, 1 }, 
            1);

        var constant = new ConstantComponent(1, 2);

        var outputConnection = new Connection(tfComponent.Id, -1);
        var inputConnection = new Connection(constant.Id, tfComponent.Id);

        var request = new SimulationRequest
        {
            Components = new List<ISimulatable> { tfComponent, constant },
            Connections = new List<Connection> {outputConnection, inputConnection},
            PreviousSignals = new Dictionary<Connection, double>()
        };

        double duration = 10.0;
        int steps = (int)(duration / request.settings.TimeStep);
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

        Assert.True(outputs.Last() > 0.9, $"Final output should approach 1.0 - {outputs.Last()}");
        Assert.True(outputs.First() < 0.2, $"Initial output should be near 0.1 or lower - {outputs.First()}");
    }

}
