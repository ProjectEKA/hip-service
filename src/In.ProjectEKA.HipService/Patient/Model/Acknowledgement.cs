namespace In.ProjectEKA.HipService.Patient.Model
{
    using Microsoft.AspNetCore.Http;

    public class Acknowledgement
    {
        public string HealthId { get; }
        public Status Status { get; }

        public Acknowledgement(string healthId, Status status)
        {
            HealthId = healthId;
            Status = status;
        }
    }
}