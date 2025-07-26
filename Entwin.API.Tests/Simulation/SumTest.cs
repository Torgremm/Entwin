using Xunit;
using Entwin.API.Models;
using Entwin.API.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Entwin.API.Controllers;
using ScottPlot;

namespace Entwin.API.Tests.Simulation;

public class SumTests
{
    [Fact]
    public void SumResponse_ShouldAddMultipleSignals(){
        double con1 = 2.34;
        double con2 = 6.87;
        double con3 = 146.06;
        double con4 = 123.3452;

        double trueResult = con1 + con2 - con3 + con4;

        var components = new List<ISimulatable>();
        var connections = new List<Connection>();

        components.Add( new ConstantComponent(con1, 2));
        components.Add( new ConstantComponent(con2, 3));
        components.Add( new ConstantComponent(con3, 4));
        components.Add( new ConstantComponent(con4, 5));

        foreach (ISimulatable comp in components)
        {
            connections.Add(new Connection(comp.Id, 6));
        }

        components.Add(new SumComponent(new List<bool>{true,true,false,true}, 6)); // con1 + con2 - con3 + con4
        //Request should sort Id ? and send boolean according to ascending Id?

        var outputConnection = new Connection(6, -1);

        connections.Add(outputConnection);

        var request = new SimulationRequest
        {
            Components = components,
            Connections = connections,
            PreviousSignals = new Dictionary<Connection, double>()
        };

        double duration = 10.0;
        int steps = (int)(duration / SimulationSettings.TimeStep);
        var outputs = new List<double>();
        double currentTime = 0.0;

        double twoSecondResult = 0;
        double eightSecondResult = 0;

        for (int i = 0; i < steps; i++)
        {
            var response = Services.CanvasSimulation.SimulateCanvas(request);

            request.PreviousSignals = response.PreviousSignals;
            currentTime = response.Time;
            double output = response.ConnectionSignals[outputConnection];
            if (currentTime - 2 < 0.1)
                twoSecondResult = output;

            if (currentTime - 8 < 0.1)
                eightSecondResult = output;

            outputs.Add(output);
        }

        Assert.True(twoSecondResult == trueResult, $"Output should match at two second mark - {twoSecondResult - trueResult}");
        Assert.True(eightSecondResult == trueResult, $"Output should match at eight second mark - {eightSecondResult - trueResult}");
    }

}
