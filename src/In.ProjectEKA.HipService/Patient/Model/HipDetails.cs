namespace In.ProjectEKA.HipService.Patient.Model
{
    public class HipDetails
    {
        public string Id { get; }
        public string CustomCode { get; }

        public HipDetails(string id, string customCode)
        {
            Id = id;
            CustomCode = customCode;
        }
    }
}