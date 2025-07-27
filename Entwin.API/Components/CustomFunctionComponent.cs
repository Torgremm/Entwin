using System.ComponentModel;
using Entwin.API.Models;
using Entwin.API.Services;

namespace Entwin.API.Components
{
    public class CustomFunctionComponent : ISimulatable
    {
        private readonly string _expressionString;
        private SimulationSettings _settings;
        public int Id { get; set; }
        private int[] _desiredInput { get; set; }

        public CustomFunctionComponent(string userExpression, int[] desiredInput, int id, SimulationRequest req)
        {
            _expressionString = userExpression;
            _desiredInput = desiredInput;

            Id = id;

            _settings = req.settings;
            ValidateExpression(_expressionString);
        }

        public double SimulateStep(double[] input, double currentTime)
        {
            CustomFunctionRequest req = new()
            {
                userExpression = _expressionString,
                time = _settings.Time
            };
            return CustomFunctionSimulation.SimulateCustomFunction(req, input);
        }

        public double[] SortedInput(double[] input, int[] Ids)
        {
            var signalById = new Dictionary<int, double>();
            for (int i = 0; i < Ids.Length; i++)
            {
                signalById[Ids[i]] = input[i];
            }

            double[] sortedSignals = new double[_desiredInput.Length];
            for (int i = 0; i < _desiredInput.Length; i++)
            {
                int id = _desiredInput[i];
                if (signalById.TryGetValue(id, out var signal))
                {
                    sortedSignals[i] = signal;
                }
                else
                {
                    sortedSignals[i] = 0.0;
                }
            }

            return sortedSignals;
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
