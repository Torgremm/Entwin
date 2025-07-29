namespace Entwin.Client.Components
{
    public class Gain : BaseComponentData
    {
        public double Value { get; set; } = 1;
        public Gain()
        {
            DisplayName = "x" + Value.ToString();
            InputCount = 1;
            OutputCount = 1;
        }
    }
}
