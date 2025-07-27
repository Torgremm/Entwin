using Entwin.API.Components;
using Entwin.API.Models;

namespace Entwin.API.Services;

public static class CanvasSimulation
{
    public static SimulationResponse SimulateCanvas(SimulationRequest req)
    {
        var outputs = req.Components.ToDictionary(
            component => component.Id,
            component => {
                var incomingConnections = req.Connections
                    .Where(c => c.To == component.Id)
                    .ToArray();
    
                double[] sortedInput = new double[incomingConnections.Length];
                foreach (Connection c in incomingConnections){
                    var signal = req.PreviousSignals.TryGetValue(c, out var s) ? s : 0.0;
                    sortedInput[c.To_Position] = signal;
                }
                return component.SimulateStep(sortedInput, req.settings.Time);
            }
        );


        var currentSignals = req.Connections
            .Where(c => outputs.ContainsKey(c.From))
            .ToDictionary(c => c, c => outputs[c.From]);

        return new SimulationResponse
        {
            Time = req.settings.StepTime(),
            ConnectionSignals = currentSignals,
            PreviousSignals = currentSignals
        };
    }
}
