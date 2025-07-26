using Entwin.API.Models;
using Entwin.API.Components;
using Entwin.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Entwin.API.Controllers;

public static class SimulationController
{
    public static void RegisterEndpoints(WebApplication app){
        app.MapPost("/simulate-step", ([FromBody] SimulationRequest req) =>
        {
            var result = CanvasSimulation.SimulateCanvas(req);
            return Results.Ok(result);
        });

        //There is no need for this?
        app.MapPost("/simulate-transfer-function", ([FromBody] TransferFunctionRequest req) =>
        {
            var result = TransferFunctionSimulation.SimulateTransferFunction(req);
            return Results.Ok(result);
        });
    }
}
