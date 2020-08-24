namespace In.ProjectEKA.FHIRHip.Link
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Discovery.Model;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using Newtonsoft.Json;
    using Optional;
    using Patient;
    using Serilog;

    public  class PatientRepository : IPatientRepository
    {
        private readonly HttpClient client;
        private readonly PatientConfiguration patientConfiguration;

        public PatientRepository(HttpClient client, PatientConfiguration patientConfiguration)
        {
            this.client = client;
            this.patientConfiguration = patientConfiguration;
        }

        public async Task<Option<Patient>> PatientWith(string referenceNumber)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, patientConfiguration.BaseUrl + patientConfiguration.PathLink)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new
                        {
                            identifier = referenceNumber
                        }),
                        Encoding.UTF8,
                        "application/json")
                };
                var response = await client.SendAsync(request);
                var responseContent = response.Content;
                using var reader = new StreamReader(await responseContent.ReadAsStreamAsync());
                var result = await reader.ReadToEndAsync().ConfigureAwait(false);
                var patientResponseResult = JsonConvert.DeserializeObject<PatientResponseResult>(result);
                var patients = patientResponseResult.Results.Select(patient => new Patient
                {
                    Name = patient.Name,
                    Gender = patient.Gender,
                    Identifier = patient.Identifier,
                    CareContexts = patient.CareContexts,
                    PhoneNumber = patient.PhoneNumber,
                    YearOfBirth = patient.YearOfBirth
                }).AsQueryable();
                return Option.Some(patients.First());
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return Option.None<Patient>();
            }
        }
    }
}