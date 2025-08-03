using Entwin.Shared.Components;
using Entwin.Shared.Models;

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

        public override SimulatableDTOBase ToDTO()
        {
            return new CustomFunctionDTO
            {
                Id = this.Id,
                UserInput = this.UserInput
            };
        }

        public CustomFunction(ComponentSaveDTO dto) : base(dto)
        {
            if (dto.SimulationData is CustomFunctionDTO data)
            {
                Id = data.Id;
                UserInput = data.UserInput;
            }
            else
            {
                throw new ArgumentException("Invalid DTO for Constant");
            }
        }
    }
}
