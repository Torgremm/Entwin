using ScottPlot;
using System;
using System.IO;

namespace Entwin.API.Tests.Simulation;
public static class PlotHelper
{
    public static void SaveLinePlot(
        double[] xs,
        double[] ys,
        string title,
        int width = 600,
        int height = 400)
    {
        var plt = new ScottPlot.Plot();

        plt.Add.Scatter(xs, ys);
        plt.Title(title);
        plt.XLabel("t");
        plt.YLabel("Y");

        string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "output.png");

        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

        plt.SavePng(outputPath, width, height);
    }
}
