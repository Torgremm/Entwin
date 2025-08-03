using Entwin.Shared.Components;
using Entwin.Shared.Models;

namespace Entwin.Client.Components
{
    public class TransferFunction : BaseComponentData
    {
        public double[] Numerator = [1];
        public double[] Denominator = [1, 1];
        public TransferFunction()
        {
            DisplayName = "1 / s + 1";
            InputCount = 1;
            OutputCount = 1;
        }

        public override SimulatableDTOBase ToDTO()
        {
            return new TransferFunctionDTO
            {
                Id = this.Id,
                sNumerator = Numerator.Select(n => (double)n).ToList(),
                sDenominator = Denominator.Select(d => (double)d).ToList()
            };
        }

        public TransferFunction(ComponentSaveDTO dto) : base(dto)
        {
            if (dto.SimulationData is TransferFunctionDTO data)
            {
                Id = data.Id;
                Numerator = data.sNumerator.ToArray();
                Denominator = data.sDenominator.ToArray();
            }
            else
            {
                throw new ArgumentException("Invalid DTO for Constant");
            }
        }
    }
}
