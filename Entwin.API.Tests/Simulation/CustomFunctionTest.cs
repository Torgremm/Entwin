using Xunit;
using Entwin.API.Models;
using Entwin.API.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Entwin.API.Controllers;
using ScottPlot;
using System.Linq.Expressions;

namespace Entwin.API.Tests.Simulation;

[Trait("Category","Simulation")]
public class CustomFunctionTests
{
    [Fact]
    public void SineWave_ShouldOutputAsExpected()
    {
        var request = new SimulationRequest
        {
            Components = new List<ISimulatable>(),
            Connections = new List<Connection>(),
            PreviousSignals = new Dictionary<Connection, double>()
        };

        string customCode = "input0 - input1 + Sin(time)";

        var con1 = 2.16;
        var con2 = 11.85;
        var con1Component = new ConstantComponent(con1, 0);
        var con2Component = new ConstantComponent(con2, 1);
        var functionComponent = new CustomFunctionComponent(customCode, 2, request); //con2 = input0, con1 = input1
        var c1 = new Connection(con1Component.Id, functionComponent.Id,0,1);
        var c2 = new Connection(con2Component.Id, functionComponent.Id,0,0);
        var outputConnection = new Connection(functionComponent.Id, -1);

        request.Components.Add(con1Component);
        request.Components.Add(con2Component);
        request.Components.Add(functionComponent);
        request.Connections.Add(c1);
        request.Connections.Add(c2);
        request.Connections.Add(outputConnection);

        double duration = 10.0;
        int steps = (int)(duration / request.settings.TimeStep);
        var outputs = new List<double>();
        double currentTime;
        double trueValue;

        for (int i = 0; i < steps; i++)
        {
            var response = Services.CanvasSimulation.SimulateCanvas(request);
            request.settings.Time = response.Time;
            request.PreviousSignals = response.PreviousSignals;
            currentTime = response.Time;
            trueValue = Math.Sin(currentTime - request.settings.TimeStep) - con1 + con2;
            double output = response.ConnectionSignals[outputConnection];
            outputs.Add(output);
            if (i == steps / 4 || i == steps / 8)
                Assert.True(Math.Abs(output - trueValue) < 1e-6, $"Unexpected expression evaluation at {currentTime} : {output} - {trueValue}");
        }
    }
    [Fact]
    public void InvalidInput_ShouldBeRejected()
    {
        try
        {
            var request = new SimulationRequest
            {
                Components = new List<ISimulatable>(),
                Connections = new List<Connection>(),
                PreviousSignals = new Dictionary<Connection, double>()
            };

            string customCode = "using.System.IO; input0 + input1 + Sin(time)";

            var con1 = 1;
            var con2 = 2;
            var con1Component = new ConstantComponent(con1, 0);
            var con2Component = new ConstantComponent(con2, 1);
            var functionComponent = new CustomFunctionComponent(customCode, 2, request);
            var c1 = new Connection(con1Component.Id, functionComponent.Id);
            var c2 = new Connection(con2Component.Id, functionComponent.Id);
            var outputConnection = new Connection(functionComponent.Id, -1);
            Assert.Fail("Illegal term detected, Error should have been caught");
        }
        catch (InvalidOperationException)
        {
            Assert.True(true, "");
        }

    }
    [Fact]
    public void NonNumericOutput_ShouldBeRejected() {
        
        var request = new SimulationRequest
        {
            Components = new List<ISimulatable>(),
            Connections = new List<Connection>(),
            PreviousSignals = new Dictionary<Connection, double>()
        };

        string customCode = "1 > 0";

        var con1 = 1;
        var con2 = 2;
        var con1Component = new ConstantComponent(con1,0);
        var con2Component = new ConstantComponent(con2,1);
        var functionComponent = new CustomFunctionComponent(customCode,2,request);
        var c1 = new Connection(con1Component.Id, functionComponent.Id);
        var c2 = new Connection(con2Component.Id, functionComponent.Id);
        var outputConnection = new Connection(functionComponent.Id, -1);

        request.Components.Add(con1Component);
        request.Components.Add(con2Component);
        request.Components.Add(functionComponent);
        request.Connections.Add(c1);
        request.Connections.Add(c2);
        request.Connections.Add(outputConnection);

        double duration = 10.0;
        int steps = (int)(duration / request.settings.TimeStep);
        var outputs = new List<double>();
        double currentTime = 0.0;

        for (int i = 0; i < steps; i++)
        {
            try
            {
                var response = Services.CanvasSimulation.SimulateCanvas(request);
                request.PreviousSignals = response.PreviousSignals;
                request.settings.Time = response.Time;
                currentTime = response.Time;
                double output = response.ConnectionSignals[outputConnection];
                outputs.Add(output);
                Assert.Fail("Returned non number, Error should have been caught");
            }
            catch(InvalidOperationException)
            {
                Assert.True(true, "");
            }
        }

    }

}
