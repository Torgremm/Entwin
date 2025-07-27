using Entwin.API.Components;
using Entwin.API.Models;
using NCalc;


namespace Entwin.API.Services;

public static class CustomFunctionSimulation
{
    public static double SimulateCustomFunction(CustomFunctionRequest req, double[] input)
    {
        string expressionString = req.userExpression;

        Expression expression = new Expression(expressionString, EvaluateOptions.IgnoreCase | EvaluateOptions.NoCache);

        // Build the input array from the componentSignals, respecting the ordering in IdToInputMap
        for (int i = 0; i < input.Length; i++)
        {
            expression.Parameters[$"input{i}"] = input[i];
        }

        expression.Parameters["time"] = req.time;

        var result = expression.Evaluate();

        if (result is double d)
        {
            if (double.IsInfinity(d) || double.IsNaN(d))
                return double.NaN;

            return d;
        }

        throw new InvalidOperationException("Expression did not return a numeric value.");
    }


    
}