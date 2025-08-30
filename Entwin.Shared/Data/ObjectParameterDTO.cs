namespace Entwin.Shared.Data;

public class ObjectParameterDTO
{
    public int Id { get; set; }
    public int? ObjectId { get; set; } = null!;
    public int? ParentId { get; set; } = null!;
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}