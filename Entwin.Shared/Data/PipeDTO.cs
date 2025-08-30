namespace Entwin.Shared.Data;

public class PipeDTO
{
    public int Id { get; set; }
    public List<int>? OutgoingIds { get; set; }
    public List<int>? IncomingIds { get; set; }
    public double Volume { get; set; }
    public string Material { get; set; } = string.Empty;
    public double Verticality { get; set; } = 0;
}