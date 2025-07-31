using Entwin.Shared.Components;

namespace Entwin.Client.Components
{
    public class TransferFunction : BaseComponentData, ISimulatableComponent
    {
        public int[] Numerator = [1];
        public int[] Denominator = [1, 1];
        public TransferFunction()
        {
            DisplayName = "1 / s + 1";
            InputCount = 1;
            OutputCount = 1;
        }

        public SimulatableDTOBase ToDTO()
        {
            return new TransferFunctionDTO
            {
                Id = this.Id,
                sNumerator = Numerator.Select(n => (double)n).ToList(),
                sDenominator = Denominator.Select(d => (double)d).ToList()
            };
        }
    }
}
