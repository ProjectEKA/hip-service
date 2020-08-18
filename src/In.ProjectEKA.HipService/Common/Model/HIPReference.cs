namespace In.ProjectEKA.HipService.Common.Model
{
    public class HIPReference
    {
        public HIPReference(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }
    }
}