namespace Entwin.Shared.Components;

public record ConnectionDTO
{
    public int From { get; set; }
    public int To { get; set; }
    public int From_Position { get; set; }
    public int To_Position { get; set; }
}