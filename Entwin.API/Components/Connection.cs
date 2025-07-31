using Entwin.Shared.Components;

namespace Entwin.API.Components;

public record Connection : IEquatable<Connection>
{
    public int From { get; set; }
    public int To { get; set; }
    public int From_Position { get; set; } = 0;
    public int To_Position { get; set; } = 0;

    public Connection(int f, int t, int from_Position, int to_Position)
    {
        From = f;
        To = t;
        From_Position = from_Position;
        To_Position = to_Position;
    }

    public Connection(ConnectionDTO c)
    {
        From = c.From;
        To = c.To;
        From_Position = c.From_Position;
        To_Position = c.To_Position;
    }
    public Connection(int f, int t) : this(f, t, 0, 0) { }

}
