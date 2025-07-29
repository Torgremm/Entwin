namespace Entwin.Client.Components
{
    public abstract class BaseComponentData
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int InputCount { get; set; }
        public int OutputCount { get; set; }
        public bool IsSelected { get; set; }
        public string DisplayName { get; set; } = "";
    }
}
