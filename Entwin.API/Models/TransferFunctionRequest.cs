namespace Entwin.API.Models;

public enum Solver{
    ODE45,
    ODE15,
    EuF1
}

public class TimeSimulation {
    public double Duration {get;set;} = 10.0;
    public double TimeStep {get;set;} = 0.1;
    public Solver OdeSolver {get;set;} = Solver.EuF1;
}

public class TransferFunctionRequest : TimeSimulation  {
    public List<double> Numerator {get;set;} = new();
    public List<double> Denominator {get;set;} = new();

    public List<double> InputHistory { get; set; } = new();
    public List<double> OutputHistory { get; set; } = new();

    public double Input { get; set; } = 1.0;
    
    public double Time { get; set; } = 0.0;
}
