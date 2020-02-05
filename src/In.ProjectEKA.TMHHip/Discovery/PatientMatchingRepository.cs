namespace In.ProjectEKA.TMHHip.Discovery
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using Newtonsoft.Json;
    using JsonSerializer = System.Text.Json.JsonSerializer;

    public class PatientMatchingRepository : IMatchingRepository
    {
        private readonly HttpClient client;

        public PatientMatchingRepository(HttpClient client)
        {
            this.client = client;
        }

        public async Task<IQueryable<HipLibrary.Patient.Model.Patient>> Where(DiscoveryRequest predicate)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:49699/patients/find");
            request.Content = new StringContent(
                JsonConvert.SerializeObject(new {mobileNumber = predicate.Patient.VerifiedIdentifiers.First().Value}),
                Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<IEnumerable<Patient>>(responseStream);
            return result.Select(patient => new HipLibrary.Patient.Model.Patient
            {
                FirstName = patient.FirstName,
                Gender = patient.Gender,
                Identifier = patient.Identifier,
                LastName = patient.LastName,
                CareContexts = new List<CareContextRepresentation>
                {
                    new CareContextRepresentation($"{patient.Identifier}",
                        $"{patient.FirstName}  {patient.LastName}")
                },
                PhoneNumber = "8340289040"
            }).AsQueryable();
        }
    }
}