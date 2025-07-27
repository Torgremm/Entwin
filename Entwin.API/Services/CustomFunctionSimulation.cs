using Entwin.API.Components;
using Entwin.API.Models;
using Flee.PublicTypes;

namespace Entwin.API.Services;

public static class CustomFunctionSimulation
{
    public static double SimulateCustomFunction(CustomFunctionRequest req, double[] input)
    {
        string expressionString = req.userExpression;

        var context = new ExpressionContext();

        context.Imports.AddType(typeof(Math));

        for (int i = 0; i < input.Length; i++){
            context.Variables[$"input{i}"] = input[i];
        }

        context.Variables["time"] = req.time;

        IDynamicExpression e;
        try
        {
            e = context.CompileDynamic(expressionString);
        }
        catch (ExpressionCompileException ex)
        {
            throw new InvalidOperationException("Invalid expression: " + ex.Message);
        }

        var result = e.Evaluate();

        if (result is double d){
            if (double.IsInfinity(d) || double.IsNaN(d))
                return double.NaN;

            return d;
        }

        throw new InvalidOperationException("Expression did not return a numeric value.");
    }
}
