using Entwin.Shared.Data;
using Entwin.Client.Services;

namespace Entwin.Client.Data;

public class ObjectParameter : ObjectParameterDTO
{
    public bool IsSelected { get; set; } = false;

    public string TryValue()
    {
        if (!ObjectId.HasValue)
            return Value;

        RigObject? obj = ObjectTableStateService.Instance?.GetById(ObjectId.Value);

        if (obj != null)
            return obj.Tag;
            
        return Value;
    }
}