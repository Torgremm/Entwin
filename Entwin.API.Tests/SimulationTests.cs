using Xunit;
using Entwin.API.Models;
using Entwin.API.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Entwin.API.Controllers;
using ScottPlot;

namespace Entwin.API.Tests;

public class SimulationTests
{
    [Fact]
    public void UnitStepResponse_ShouldApproximateFirstOrderRise()
    {
        // First-order system: H(s) = 1 / (s + 1)
        var req = new TransferFunctionRequest
        {
            Numerator = new List<double> { 1 },
            Denominator = new List<double> { 1, 1 },
            TimeStep = 0.1
            //Input = 1.0 Default
        };

        double duration = 10.0;
        int steps = (int)(duration / req.TimeStep);

        var outputs = new List<double>();
        for (int i = 0; i < steps; i++)
        {
            var result = InvokeSimulate(req);
            req.Time = result.Time;
            req.OutputHistory = result.OutputHistory;
            req.InputHistory = result.InputHistory;
            outputs.Add(result.Output);
        }

        if (!(outputs.Last() > 0.9))
        {
            double[] times = Enumerable.Range(0, outputs.Count)
                                       .Select(i => i * req.TimeStep)
                                       .Select(x => (double)x)
                                       .ToArray();
            double[] yValues = outputs.ToArray();

            var myPlot = new ScottPlot.Plot();
            myPlot.Add.Scatter(times, yValues);

            string projectDir = AppContext.BaseDirectory;
            string solutionRoot = Path.GetFullPath(Path.Combine(projectDir, "..", "..", ".."));
            string outputPath = Path.Combine(solutionRoot, "output.png");
            myPlot.SavePng(outputPath, 600, 400);

            Assert.Fail($"Final output should approach 1.0 - {outputs.Last()}. Plot saved to output.png");
        }


        Assert.True(outputs.First() < 0.2, $"Initial output should be near 0.1 or lower - {outputs.First()}");
    }

    [Fact]
    public void UnitStepResponse_ShouldApproximateFourthOrderRise()
    {
        // First-order system: H(s) = 1 / (s + 1)
        var req = new TransferFunctionRequest
        {
            Numerator = new List<double> { 1, 0.2 },
            Denominator = new List<double> { 1, 2, 5, 2, 1 },
            TimeStep = 0.1
            //Input = 1.0 Default
        };

        double duration = 20.0;
        int steps = (int)(duration / req.TimeStep);

        var outputs = new List<double>();
        for (int i = 0; i < steps; i++)
        {
            var result = InvokeSimulate(req);
            req.Time = result.Time;
            req.OutputHistory = result.OutputHistory;
            req.InputHistory = result.InputHistory;
            outputs.Add(result.Output);
        }

        if (!(outputs.Last() > 0.9 && outputs.Last() < 1.1))
        {
            double[] times = Enumerable.Range(0, outputs.Count)
                                       .Select(i => i * req.TimeStep)
                                       .Select(x => (double)x)
                                       .ToArray();
            double[] yValues = outputs.ToArray();

            var myPlot = new ScottPlot.Plot();
            myPlot.Add.Scatter(times, yValues);

            string projectDir = AppContext.BaseDirectory;
            string solutionRoot = Path.GetFullPath(Path.Combine(projectDir, "..", "..", ".."));
            string outputPath = Path.Combine(solutionRoot, "output.png");
            myPlot.SavePng(outputPath, 600, 400);

            Assert.Fail($"Final output should approach 1.0 - {outputs.Last()}. Plot saved to output.png");
        }


        Assert.True(outputs.First() < 0.2, $"Initial output should be near 0.1 or lower - {outputs.First()}");
    }

    [Fact]
    public void StepSignal_ShouldMatchExpectedValues_At5And10Seconds(){
        // Arrange
        var stepComponent = new StepComponent
        {
            StartValue = 0.3,
            EndValue = 1.0,
            SwitchTime = 5.0,
            Id = 100
        };

        var transferFunction = new TransferFunctionComponent
        {
            Id = 200,
            Numerator = new List<double> { 1.0 },
            Denominator = new List<double> { 1.0, 1.0 }
        };

        var connection = new Connection { From = 100, To = 200 };

        var request = new SimulationRequest
        {
            TimeKeeper = new TimeSimulation
            {
                Duration = 10.0,
                TimeStep = 1.0,
                OdeSolver = Solver.EuF1
            },
            Components = new List<ISimulatable> { stepComponent, transferFunction },
            Connections = new List<Connection> { connection },
            PreviousSignals = new Dictionary<Connection, double>()
        };

        double t = 0.0;
        double outputAt5 = 0.0;
        double outputAt10 = 0.0;

        while (t < 10.0){
            var response = InvokeSimulate(request);

            if (Math.Abs(response.Time - 5.0) < 0.1)
                outputAt5 = response.ConnectionSignals[connection];

            if (Math.Abs(response.Time - 10.0) < 0.1)
                outputAt10 = response.ConnectionSignals[connection];

            // Prepare request for next step
            request.PreviousSignals = response.PreviousSignals;
            request.TimeKeeper.Time = response.Time;
            t = response.Time;
        }

        Assert.Equal(0.3, outputAt5, 3);
        Assert.Equal(1.0, outputAt10, 3);
    }



    private TransferFunctionResponse InvokeSimulate(TransferFunctionRequest req)
    {
        var simulationControllerType = typeof(SimulationController);

        var methodInfo = simulationControllerType.GetMethod(
        "SimulateTransferFunction",
        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

        if (methodInfo == null)
            throw new InvalidOperationException("Method 'SimulateTransferFunction' not found on SimulationController.");

        var result = methodInfo.Invoke(null, new object[] { req });

        return result as TransferFunctionResponse ?? throw new InvalidOperationException("Simulation method did not return expected type.");
    }
    
    private SimulationResponse InvokeSimulate(SimulationRequest req)
    {
        var simulationControllerType = typeof(SimulationController);

        var methodInfo = simulationControllerType.GetMethod(
        "SimulateCanvas",
        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

        if (methodInfo == null)
            throw new InvalidOperationException("Method 'SimulateCanvas' not found on SimulationController.");

        var result = methodInfo.Invoke(null, new object[] { req });

        return result as SimulationResponse ?? throw new InvalidOperationException("Simulation method did not return expected type.");
    }

}
