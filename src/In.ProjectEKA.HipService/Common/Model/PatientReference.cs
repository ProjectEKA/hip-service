namespace In.ProjectEKA.HipService.Common.Model
{
    public class PatientReference
    {
        public PatientReference(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}