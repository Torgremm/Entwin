using Entwin.Shared.Components;

namespace Entwin.Client.Components
{
    public class Constant : BaseComponentData, ISimulatableComponent
    {
        public double Value { get; set; } = 1;
        public Constant()
        {
            DisplayName = Value.ToString();
            InputCount = 0;
            OutputCount = 1;
        }

        public SimulatableDTOBase ToDTO()
        {
            return new ConstantDTO
            {
                Id = this.Id,
                Value = this.Value
            };
        }
    }
}
