using Entwin.Shared.Components;
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

        public SumDTO ToDTO()
        {
            return new SumDTO
            {
                Id = this.Id,
                Signs = this.Signs
            };
        }
    }
}
