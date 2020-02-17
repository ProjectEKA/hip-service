namespace In.ProjectEKA.HipService.Common.Model
{
    public class PatientReference
    {
        public string Id { get; }

        public PatientReference(string id)
        {
            Id = id;
        }
    }
}