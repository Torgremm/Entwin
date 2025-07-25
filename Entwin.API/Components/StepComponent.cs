using Entwin.API.Models;
using Entwin.API.Controllers;
namespace Entwin.API.Components;

public class StepComponent : TimeSimulation, ISimulatable
{
    public double StartValue { get; set; } = new();
    public double EndValue { get; set; } = new();
    public double SwitchTime { get; set; } = new();
    public int Id { get; set; } = new();
    public double SimulateStep(double input, double currentTime){
        return currentTime > SwitchTime ? EndValue : StartValue;
    }
}