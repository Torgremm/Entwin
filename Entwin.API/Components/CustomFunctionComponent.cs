using System.ComponentModel;
using Entwin.API.Models;
using Entwin.Shared.Components;
using Entwin.API.Services;

namespace Entwin.API.Components
{
    public class CustomFunctionComponent : ISimulatable
    {
        private readonly string _expressionString;
        public int Id { get; set; }

        public CustomFunctionComponent(string userExpression, int id, SimulationRequest req)
        {
            _expressionString = userExpression;

            Id = id;

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
            };
            return [CustomFunctionSimulation.SimulateCustomFunction(req, input, currentTime)];
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
