namespace Entwin.API.Components;

public record Connection : IEquatable<Connection>
{
    public int From { get; set; }
    public int To { get; set; }

    public Connection(int f, int t)
    {
        From = f;
        To = t;
    }
    
}
