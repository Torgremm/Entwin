using Entwin.Shared.Components;

namespace Entwin.Client.Components
{
    public class CustomFunction : BaseComponentData, ISimulatableComponent
    {
        public string UserInput { get; set; } = "input0";
        public CustomFunction()
        {
            DisplayName = "Y = f(x)";
            InputCount = 1;
            OutputCount = 1;
        }

        public SimulatableDTOBase ToDTO()
        {
            return new CustomFunctionDTO
            {
                Id = this.Id,
                UserInput = this.UserInput
            };
        }
    }
}
