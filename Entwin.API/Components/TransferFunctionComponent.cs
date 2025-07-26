using Entwin.API.Models;
using Entwin.API.Services;

namespace Entwin.API.Components;

public class TransferFunctionComponent : ISimulatable
{
    public int Id { get; set; } = new();
    public List<double> sNumerator { get; set; } = new();
    public List<double> sDenominator { get; set; } = new();

    private List<double> zNumerator { get; set; }
    private List<double> zDenominator { get; set; }
    private List<double> _inputHistory = new();
    private List<double> _outputHistory = new();
    private double _currentTime = 0.0;

    public TransferFunctionComponent(List<double> num, List<double> den, int id)
    {
        Id = id;
        var (b, a) = DiscretizeEuler(num, den, SimulationSettings.TimeStep);
        zNumerator = b.ToList();
        zDenominator = a.ToList();
    }
    public double SimulateStep(double[] input, double currentTime)
    {
        _currentTime = currentTime;

        var tfRequest = new TransferFunctionRequest
        {
            Numerator = this.zNumerator,
            Denominator = this.zDenominator,
            Input = input[0],
            InputHistory = _inputHistory,
            OutputHistory = _outputHistory
        };

        var response = TransferFunctionSimulation.SimulateTransferFunction(tfRequest);

        _inputHistory = response.InputHistory;
        _outputHistory = response.OutputHistory;

        return response.Output;
    }

    private static (double[] b_z, double[] a_z) DiscretizeEuler(List<double> b_s, List<double> a_s, double dt)
    {
        var num_z = ConvertPolynomial(b_s.ToArray(), dt);
        var den_z = ConvertPolynomial(a_s.ToArray(), dt);

        // Normalize so that a_z[0] = 1
        double a0 = den_z[0];
        for (int i = 0; i < num_z.Length; i++) num_z[i] /= a0;
        for (int i = 0; i < den_z.Length; i++) den_z[i] /= a0;

        return (num_z, den_z);
    }

    private static double[] ConvertPolynomial(double[] sCoeffs, double dt)
    {
        var result = new List<double> { 0.0 };

        for (int i = 0; i < sCoeffs.Length; i++)
        {
            // Compute ((z - 1)/dt)^i 
            //TODO: Pass solver here
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

    private static List<double> AddPolynomials(List<double> a, List<double> b)
    {
        int n = Math.Max(a.Count, b.Count);
        var result = new List<double>(new double[n]);

        for (int i = 0; i < a.Count; i++)
            result[i] += a[i];
        for (int i = 0; i < b.Count; i++)
            result[i] += b[i];

        return result;
    }

    private static List<double> MultiplyPolynomials(List<double> a, List<double> b)
    {
        var result = new double[a.Count + b.Count - 1];

        for (int i = 0; i < a.Count; i++)
            for (int j = 0; j < b.Count; j++)
                result[i + j] += a[i] * b[j];

        return result.ToList();
    }
}