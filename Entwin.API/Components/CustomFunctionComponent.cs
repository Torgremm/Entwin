using System.ComponentModel;
using Entwin.API.Models;
using Entwin.Shared.Components;
using Entwin.API.Services;

namespace Entwin.API.Components
{
    public class CustomFunctionComponent : ISimulatable
    {
        private readonly string _expressionString;
        private SimulationSettings _settings;
        public int Id { get; set; }

        public CustomFunctionComponent(string userExpression, int id, SimulationRequest req)
        {
            _expressionString = userExpression;

            Id = id;

            _settings = req.settings;
            ValidateExpression(_expressionString);
        }
        public CustomFunctionComponent(CustomFunctionDTO func)
    {
        Id = func.Id;
        _expressionString = func.UserInput;
    }

        public double[] SimulateStep(double[] input, double currentTime)
        {
            CustomFunctionRequest req = new()
            {
                userExpression = _expressionString,
                time = _settings.Time
            };
            return [CustomFunctionSimulation.SimulateCustomFunction(req, input)];
        }

        private void ValidateExpression(string expr)
        {
            string[] blacklist = {
                "Process", "System.", "File", "typeof", "new", "Environment", "Console", "delegate"
            };

            foreach (var bad in blacklist)
            {
                if (expr.Contains(bad, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"Use of '{bad}' is not allowed in expressions.");
            }
        }
    }
}
