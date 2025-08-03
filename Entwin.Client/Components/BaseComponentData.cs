using Entwin.Shared.Components;
using Entwin.Shared.Models;

namespace Entwin.Client.Components
{
    public abstract class BaseComponentData
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int InputCount { get; set; }
        public int OutputCount { get; set; }
        public bool IsSelected { get; set; }
        public string DisplayName { get; set; } = "";

        public virtual ComponentSaveDTO ToSave()
        {
            return new ComponentSaveDTO
            {
                SimulationData = this.ToDTO(),
                VisualData = new UIData
                {
                    X = this.X,
                    Y = this.Y,
                    InputCount = this.InputCount,
                    OutputCount = this.OutputCount
                }
            };
        }

        public abstract SimulatableDTOBase ToDTO();
    }
}
