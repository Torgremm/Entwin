using Entwin.Shared.Components;
using Entwin.Shared.Models;

namespace Entwin.Client.Components
{
    public class Sum : BaseComponentData
    {
        public string Signs = "++";
        public Sum()
        {
            DisplayName = "+";
            InputCount = 2;
            OutputCount = 1;
        }

        public override SimulatableDTOBase ToDTO()
        {
            return new SumDTO
            {
                Id = this.Id,
                Signs = this.Signs
            };
        }

        public Sum(ComponentSaveDTO dto) : base(dto)
        {
            if (dto.SimulationData is SumDTO data)
            {
                Id = data.Id;
                Signs = data.Signs ?? "++";
            }
            else
            {
                throw new ArgumentException("Invalid DTO for Constant");
            }
        }
    }
}
