using Microsoft.AspNetCore.Mvc;
using Entwin.API.Models;
using Entwin.API.Components;
using Entwin.API.Services;
using Entwin.Shared.Models;
using Entwin.Shared.Components;

namespace Entwin.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SimulationController : ControllerBase
{
    [HttpPost("simulate-step")]
    public IActionResult SimulateStep([FromBody] SimulationRequestDTO reqDTO)
    {
        var settings = reqDTO.Settings;

        var components = reqDTO.Components
            .Select(dto => ComponentMapper.ConvertDTO(dto, reqDTO))
            .ToList();

        var connections = reqDTO.Connections
            .Select(dto => new Connection(dto))
            .ToList();

        var initialSignals = connections.ToDictionary(conn => conn, _ => 0.0);

        var internalRequest = new SimulationRequest
        {
            settings = new SimulationSettings(settings),
            Components = components,
            Connections = connections,
            PreviousSignals = initialSignals
        };

        var result = CanvasSimulation.SimulateCanvas(internalRequest);

        var dto = new SimulationResultDTO
        {
            Time = result.Time,
            Signals = result.ConnectionSignals.ToDictionary(
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
