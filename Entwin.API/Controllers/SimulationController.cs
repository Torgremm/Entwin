using Microsoft.AspNetCore.Mvc;
using Entwin.API.Models;
using Entwin.API.Components;
using Entwin.API.Services;
using Entwin.Shared.Models;
using Entwin.Shared.Components;
using Microsoft.AspNetCore.Authorization;

namespace Entwin.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class SimulationController : ControllerBase
{
    [HttpPost("simulate-step")]
    public IActionResult SimulateStep([FromBody] SimulationRequestDTO reqDTO)
    {
        var settings = new SimulationSettings(reqDTO.Settings);
        double duration = reqDTO.Settings.Duration;
        double timeStep = reqDTO.Settings.TimeStep; 

        var components = reqDTO.Components
            .Select(dto => ComponentMapper.ConvertDTO(dto, reqDTO))
            .ToList();

        var connections = reqDTO.Connections
            .Select(dto => new Connection(dto))
            .ToList();

        var previousSignals = connections.ToDictionary(conn => conn, _ => 0.0);
        var internalRequest = new SimulationRequest
        {
            settings = settings,
            Components = components,
            Connections = connections,
            PreviousSignals = previousSignals
        };

        var timeSteps = new List<double>();
        var simulationResults = connections.ToDictionary(conn => conn, _ => new List<double>());

        while (internalRequest.settings.Time < duration)
        {
            timeSteps.Add(internalRequest.settings.Time);
            var stepResult = CanvasSimulation.SimulateCanvas(internalRequest);

            foreach (var conn in connections)
            {
                stepResult.ConnectionSignals.TryGetValue(conn, out var signal);
                simulationResults[conn].Add(signal);
            }

            internalRequest.PreviousSignals = stepResult.ConnectionSignals;
            internalRequest.settings.Time += timeStep;
        }


        var dto = new SimulationResultDTO
        {
            Time = timeSteps,
    
            Signals = simulationResults.ToDictionary(
                kvp => new SignalKey(
                    kvp.Key.From,
                    kvp.Key.From_Position,
                    kvp.Key.To,
                    kvp.Key.To_Position
                ),
                kvp => kvp.Value
            )
        };

        return Ok(dto);
    }
}
