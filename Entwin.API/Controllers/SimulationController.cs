using Entwin.API.Models;
using Entwin.API.Components;
using Entwin.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Entwin.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public static class SimulationController
{
    public static void RegisterEndpoints(WebApplication app){
        app.MapPost("/simulate-step", ([FromBody] SimulationRequest req) =>
        {
            var result = CanvasSimulation.SimulateCanvas(req);
            return Results.Ok(result);
        });
    }
}
