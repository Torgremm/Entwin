using Entwin.API.Components;
using Entwin.API.Models;

namespace Entwin.API.Services;

public static class CanvasSimulation
{
    public static SimulationResponse SimulateCanvas(SimulationRequest req){

        var componentById = req.Components.ToDictionary(c => c.Id);
        var outputs = new Dictionary<int, double>();

        foreach (var component in req.Components){
            // Find input connection
            var incoming = req.Connections.FirstOrDefault(c => c.To == component.Id);

            double input = 0.0;
            if (incoming != null && req.PreviousSignals.TryGetValue(incoming, out var signal)){
                input = signal;
            }

            double output = component.SimulateStep(input, SimulationSettings.Time);
            outputs[component.Id] = output;
        }

        var currentSignals = new Dictionary<Connection, double>();
        foreach (var conn in req.Connections){
            if (outputs.TryGetValue(conn.From, out double output)){
                currentSignals[conn] = output;
            }
        }

        return new SimulationResponse
        {
            Time = SimulationSettings.stepTime(),
            ConnectionSignals = currentSignals,
            PreviousSignals = currentSignals
        };
    }
}