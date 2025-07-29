namespace Entwin.Client.Components
{
    public class CustomFunction : BaseComponentData
    {
        public string UserInput { get; set; } = "input0";
        public CustomFunction()
        {
            DisplayName = "Y = f(x)";
            InputCount = 1;
            OutputCount = 1;
        }
    }
}
