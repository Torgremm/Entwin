using Entwin.Shared.Components;

namespace Entwin.Client.Components;

public interface ISimulatableComponent
{
    SimulatableDTOBase ToDTO();
}
