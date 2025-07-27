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
            var inputsWithIds = req.Connections
                .Where(c => c.To == component.Id) //Incoming signals
                .Select(c => new { //Find IDs of components
                    Id = c.From,
                    Signal = req.PreviousSignals.TryGetValue(c, out var signal) ? signal : 0.0
                })
                .ToArray();

            var rawInput = inputsWithIds.Select(x => x.Signal).ToArray();
            var rawIds = inputsWithIds.Select(x => x.Id).ToArray();
            var sortedInput = component.SortedInput(rawInput, rawIds);
            return component.SimulateStep(sortedInput, req.settings.Time);
            });


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
