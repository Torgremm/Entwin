using System.Text.Json.Serialization;

namespace Entwin.Shared.Components;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(TransferFunctionDTO), "transferFunction")]
[JsonDerivedType(typeof(CustomFunctionDTO), "customFunction")]
[JsonDerivedType(typeof(GainDTO), "gain")]
[JsonDerivedType(typeof(SumDTO), "sum")]
[JsonDerivedType(typeof(StepDTO), "step")]
[JsonDerivedType(typeof(ConstantDTO), "constant")]
public abstract class SimulatableDTOBase { }
