using Entwin.API.Models;
using Entwin.API.Components;
using Entwin.API.Services;
using Entwin.Shared.Components;
using Entwin.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace Entwin.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public static class SimulationController
{
    public static void RegisterEndpoints(WebApplication app){
        app.MapPost("/simulate-step", ([FromBody] SimulationRequestDTO reqDTO) =>
        {
            var settings = reqDTO.Settings;

            var components = reqDTO.Components
                .Select(dto => ComponentMapper.ConvertDTO(dto, reqDTO))
                .ToList();

            var cons = new List<Connection>();

            foreach (ConnectionDTO c in reqDTO.Connections)
            {
                cons.Add(new Connection(c));
            }

            var initialSignals = cons.ToDictionary(conn => conn, _ => 0.0);


            var internalRequest = new SimulationRequest
            {
                settings = new SimulationSettings(settings),
                Components = components,
                Connections = cons,
                PreviousSignals = initialSignals
            };

            var result = CanvasSimulation.SimulateCanvas(internalRequest);
            return Results.Ok(result);
        });

    }
}
