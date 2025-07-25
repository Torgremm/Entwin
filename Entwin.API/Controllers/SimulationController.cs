using Entwin.API.Models;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.AspNetCore.Mvc;

namespace Entwin.API.Controllers;

public static class SimulationController
{
    public static void RegisterEndpoints(WebApplication app)
    {
        app.MapPost("/simulate-step", ([FromBody] TransferFunctionRequest req) =>
        {
            var result = SimulateTransferFunction(req);
            return Results.Ok(result);
        });
    }

    private static TransferFunctionResponse SimulateTransferFunction(TransferFunctionRequest req){
        int order = req.Denominator.Count - 1;
        if (order < 0)
            throw new ArgumentException("Denominator must be at least order 0.");

        var (b, a) = DiscretizeEuler(req.Numerator, req.Denominator, req.TimeStep);

        double u_n = req.Input;  // current input at this timestep

        var inputHistory = req.InputHistory ?? new List<double>();
        var outputHistory = req.OutputHistory ?? new List<double>();

        // Append current input to history
        inputHistory.Add(u_n);

        // Pad histories with zeros if needed
        while (inputHistory.Count < b.Length)
            inputHistory.Insert(0, 0.0);
        while (outputHistory.Count < order)
            outputHistory.Insert(0, 0.0);

        // Compute output y[n] using difference equation
        double y_n = 0.0;

        for (int i = 0; i < b.Length; i++)
        {
            int inputIndex = inputHistory.Count - 1 - i;
            if (inputIndex >= 0)
                y_n += b[i] * inputHistory[inputIndex];
        }

        for (int i = 1; i < a.Length; i++)
        {
            int outputIndex = outputHistory.Count - i;
            if (outputIndex >= 0)
                y_n -= a[i] * outputHistory[outputIndex];
        }

        outputHistory.Add(y_n);

        return new TransferFunctionResponse
        {
            Time = req.Time + req.TimeStep,
            Output = y_n,
            InputHistory = inputHistory,
            OutputHistory = outputHistory
        };
    }

    private static (double[] b_z, double[] a_z) DiscretizeEuler(List<double> b_s, List<double> a_s, double dt){
        var num_z = ConvertPolynomial(b_s.ToArray(), dt);
        var den_z = ConvertPolynomial(a_s.ToArray(), dt);

        // Normalize so that a_z[0] = 1
        double a0 = den_z[0];
        for (int i = 0; i < num_z.Length; i++) num_z[i] /= a0;
        for (int i = 0; i < den_z.Length; i++) den_z[i] /= a0;

        return (num_z, den_z);
    }

    private static double[] ConvertPolynomial(double[] sCoeffs, double dt){
        var result = new List<double> { 0.0 };

        for (int i = 0; i < sCoeffs.Length; i++)
        {
            // Compute ((z - 1)/dt)^i
            var basePoly = new List<double> { 1.0, -1.0 }; // (z - 1)
            var term = new List<double> { 1.0 };

            for (int j = 0; j < i; j++)
                term = MultiplyPolynomials(term, basePoly);

            // Scale by (1/dt)^i
            double scale = sCoeffs[i] / Math.Pow(dt, i);
            for (int k = 0; k < term.Count; k++)
                term[k] *= scale;

            result = AddPolynomials(result, term);
        }

        return result.ToArray();
    }

    private static List<double> AddPolynomials(List<double> a, List<double> b){
        int n = Math.Max(a.Count, b.Count);
        var result = new List<double>(new double[n]);

        for (int i = 0; i < a.Count; i++)
            result[i] += a[i];
        for (int i = 0; i < b.Count; i++)
            result[i] += b[i];

        return result;
    }

    private static List<double> MultiplyPolynomials(List<double> a, List<double> b){
        var result = new double[a.Count + b.Count - 1];

        for (int i = 0; i < a.Count; i++)
            for (int j = 0; j < b.Count; j++)
                result[i + j] += a[i] * b[j];

        return result.ToList();
    }

}
