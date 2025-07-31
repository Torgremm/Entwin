namespace Entwin.Shared.Components
{
    public class StepDTO : ISimulatableDTO
    {  
        public int Id { get; set; }
        public double StartValue { get; set; }
        public double EndValue { get; set; }
        public double SwitchTime { get; set; }
    }
}
