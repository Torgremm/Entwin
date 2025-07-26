using Entwin.API.Components;
using Entwin.API.Models;

namespace Entwin.API.Services;

public static class TransferFunctionSimulation
{
    public static TransferFunctionResponse SimulateTransferFunction(TransferFunctionRequest req)
    {
        int order = req.Denominator.Count - 1;
        if (order < 0)
            throw new ArgumentException("Denominator must be at least order 0.");

        var b = req.Numerator.ToArray();
        var a = req.Denominator.ToArray();

        double u_n = req.Input;

        var inputHistory = req.InputHistory ?? new List<double>();
        var outputHistory = req.OutputHistory ?? new List<double>();

        inputHistory.Add(u_n);

        // Pad histories with zeros if needed
        while (inputHistory.Count < b.Length)
            inputHistory.Insert(0, 0.0);
        while (outputHistory.Count < order)
            outputHistory.Insert(0, 0.0);

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
            Output = y_n,
            InputHistory = inputHistory,
            OutputHistory = outputHistory
        };
    }

    
}