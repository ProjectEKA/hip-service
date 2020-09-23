namespace In.ProjectEKA.FHIRHip.Discovery
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using HipLibrary.Matcher;
    using HipLibrary.Patient.Model;
    using Model;
    using Newtonsoft.Json;
    using Serilog;

    public class PatientMatchingRepository : IMatchingRepository
    {
        private readonly HttpClient client;
        private readonly PatientConfiguration patientConfiguration;


        public PatientMatchingRepository(HttpClient client, PatientConfiguration patientConfiguration)
        {
            this.client = client;
            this.patientConfiguration = patientConfiguration;
        }

        public async Task<IQueryable<Patient>> Where(DiscoveryRequest predicate)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, patientConfiguration.BaseUrl + patientConfiguration.PathDiscovery)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new
                        {
                            mobileNumber = predicate.Patient.VerifiedIdentifiers.First().Value
                        }),
                        Encoding.UTF8,
                        "application/json")
                };
                var response = await client.SendAsync(request);
                var responseContent = response.Content;
                using var reader = new StreamReader(await responseContent.ReadAsStreamAsync());
                var result = await reader.ReadToEndAsync().ConfigureAwait(false);
                var patientResponseResult = JsonConvert.DeserializeObject<PatientResponseResult>(result);
                return patientResponseResult.Results.Select(patient => new Patient
                {
                    Name = patient.Name,
                    Gender = patient.Gender,
                    Identifier = patient.Identifier,
                    CareContexts = patient.CareContexts,
                    PhoneNumber = patient.PhoneNumber,
                    YearOfBirth = patient.YearOfBirth
                }).AsQueryable();

            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }
    }
}