using Entwin.API.Components;
using Entwin.Shared.Components;
using Entwin.Shared.Models;

namespace Entwin.API.Models
{
    public static class ComponentMapper
    {
        public static ISimulatable ConvertDTO(SimulatableDTOBase dto, SimulationRequestDTO req)
        {
            return dto switch
            {
                TransferFunctionDTO tf => new TransferFunctionComponent(tf, req.Settings),
                CustomFunctionDTO cf => new CustomFunctionComponent(cf),
                ConstantDTO c => new ConstantComponent(c),
                SumDTO s => new SumComponent(s),
                GainDTO g => new GainComponent(g),
                StepDTO st => new StepComponent(st),
                _ => throw new NotSupportedException($"Unsupported DTO type: {dto.GetType().Name}")
            };
        }

        public static ProjectSaveDTO MapToDTO(ProjectModel model)
        {
            return new ProjectSaveDTO
            {
                Name = model.Name,
                CanvasData = model.CanvasData,
                SavedTime = model.SavedTime
            };
        }

        public static ProjectModel MapToModel(ProjectSaveDTO dto)
        {
            return new ProjectModel
            {
                Name = dto.Name,
                SavedTime = dto.SavedTime,
                CanvasData = dto.CanvasData
            };
        }
    }
}
