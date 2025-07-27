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


        var currentSignals = req.Connections.ToDictionary(
        c => c,
        c => {
            if (outputs.TryGetValue(c.From, out var outputArray) &&
                c.From_Position >= 0 &&
                c.From_Position < outputArray.Length){
                return outputArray[c.From_Position];
            }

            return 0.0;
        });

        return new SimulationResponse
        {
            Time = req.settings.StepTime(),
            ConnectionSignals = currentSignals,
            PreviousSignals = currentSignals
        };
    }
}
