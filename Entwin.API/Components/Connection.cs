using Entwin.API.Models;

namespace Entwin.API.Components;

public class Connection : IEquatable<Connection>
{
    public int From { get; set; }
    public int To { get; set; }

    public Connection(int f, int t)
    {
        From = f;
        To = t;
    }
    public override bool Equals(object obj) => Equals(obj as Connection);
    public bool Equals(Connection other) => other != null && From == other.From && To == other.To;
    public override int GetHashCode() => HashCode.Combine(From, To);
    
}
