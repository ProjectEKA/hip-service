namespace In.ProjectEKA.TMHHip.Link
{
    using System.Net.Http;
    using System.Text;
    using System.Collections.Generic;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using Optional;
    using Newtonsoft.Json;
    using JsonSerializer = System.Text.Json.JsonSerializer;

    public class PatientRepository : IPatientRepository
    {
        private readonly HttpClient client;

        public PatientRepository(HttpClient client)
        {
            this.client = client;
        }

        public Option<Patient> PatientWith(string referenceNumber)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://tmc.gov.in/tmh_ncg_api/patients/get");
            request.Content = new StringContent(
                JsonConvert.SerializeObject(new {caseId = referenceNumber}),
                Encoding.UTF8, "application/json");
            var response = client.SendAsync(request).Result;
            var responseStream = response.Content.ReadAsStringAsync().Result;
            var patient = JsonSerializer.Deserialize<Discovery.Patient>(responseStream);
            return Option.Some(new Patient
            {
                Name = $"{patient.FirstName} {patient.LastName}",
                CareContexts = new List<CareContextRepresentation>
                {
                    new CareContextRepresentation($"{patient.Identifier}", $"{patient.FirstName}  {patient.LastName}")
                },
                Gender = patient.Gender,
                Identifier = patient.Identifier,
                PhoneNumber = patient.PhoneNumber
            });
        }
    }
}