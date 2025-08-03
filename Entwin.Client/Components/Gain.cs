using Entwin.Shared.Components;
using Entwin.Shared.Models;

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

        public override SimulatableDTOBase ToDTO()
        {
            return new GainDTO
            {
                Id = this.Id,
                Value = this.Value
            };
        }

        public Gain(ComponentSaveDTO dto) : base(dto)
        {
            if (dto.SimulationData is GainDTO data)
            {
                Id = data.Id;
                Value = data.Value;
            }
            else
            {
                throw new ArgumentException("Invalid DTO for Constant");
            }
        }
    }
}
