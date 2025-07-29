namespace Entwin.Client.Components
{
    public class TransferFunction : BaseComponentData
    {
        public int[] Numerator = [1];
        public int[] Denominator = [1,1];
        public TransferFunction()
        {
            DisplayName = "1 / s + 1";
            InputCount = 1;
            OutputCount = 1;
        }
    }
}
