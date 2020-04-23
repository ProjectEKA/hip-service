namespace In.ProjectEKA.HipService.DataFlow
{
    public class Notifier
    {
        public Type Type { get; }
        public string Id { get; }
        
        public Notifier(Type type, string id)
        {
            Type = type;
            Id = id;
        }
    }
}