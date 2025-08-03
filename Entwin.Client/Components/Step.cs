using Entwin.Shared.Components;
namespace Entwin.Client.Components
{
    public class Step : BaseComponentData
    {
        public double StartValue { get; set; } = 0;
        public double EndValue { get; set; } = 1;
        public double SwitchTime { get; set; } = 1;
        public Step()
        {
            DisplayName = StartValue.ToString() + "/" + EndValue.ToString();
            InputCount = 0;
            OutputCount = 1;
        }

        public override SimulatableDTOBase ToDTO()
        {
            return new StepDTO
            {
                Id = this.Id,
                StartValue = this.StartValue,
                EndValue = this.EndValue,
                SwitchTime = this.SwitchTime
            };
        }
    }
}
