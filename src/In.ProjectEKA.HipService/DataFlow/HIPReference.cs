namespace In.ProjectEKA.HipService.DataFlow
{
    public class HIPReference
    {
        public string Id { get; }
        public string Name { get; }

        public HIPReference(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}