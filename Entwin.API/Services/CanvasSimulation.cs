using Entwin.API.Components;
using Entwin.API.Models;

namespace Entwin.API.Services;

public static class CanvasSimulation
{
    public static SimulationResponse SimulateCanvas(SimulationRequest req)
    {
        var outputs = req.Components.ToDictionary(
            component => component.Id,
            component =>
            {
                var input = req.Connections
                    .Where(c => c.To == component.Id)
                    .Select(c => req.PreviousSignals.TryGetValue(c, out var signal) ? signal : 0.0)
                    .ToArray();

                return component.SimulateStep(input, req.settings.Time);
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
