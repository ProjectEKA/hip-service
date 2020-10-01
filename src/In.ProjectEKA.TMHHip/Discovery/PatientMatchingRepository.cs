namespace In.ProjectEKA.TMHHip.Discovery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using HipLibrary.Matcher;
    using HipLibrary.Patient.Model;
    using Newtonsoft.Json;
    using Serilog;
    using JsonSerializer = System.Text.Json.JsonSerializer;

    public class PatientMatchingRepository : IMatchingRepository
    {
        private readonly HttpClient client;

        public PatientMatchingRepository(HttpClient client)
        {
            this.client = client;
            this.client.Timeout = TimeSpan.FromSeconds(20);
        }

        public async Task<IQueryable<HipLibrary.Patient.Model.Patient>> Where(DiscoveryRequest predicate)
        {
            static string RemoveCountryCodeFrom(string value)
            {
                return value?.Split("-").Length > 1 ? value.Split("-")[1] : value;
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://tmc.gov.in/tmh_ncg_api/patients/find")
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new
                        {
                            mobileNumber =
                                RemoveCountryCodeFrom(predicate.Patient.VerifiedIdentifiers.First().Value)
                        }),
                        Encoding.UTF8,
                        "application/json")
                };
                var response = await client.SendAsync(request);
                await using var responseStream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<IEnumerable<Patient>>(responseStream);
                Log.Information($"result from TMH backend: {result}");

                var patients = new List<HipLibrary.Patient.Model.Patient>();
                foreach (var patientFromTmh in result)
                {
                    var patient = new HipLibrary.Patient.Model.Patient
                    {
                        Identifier = patientFromTmh.Identifier,
                        Name = $"{patientFromTmh.FirstName} {patientFromTmh.LastName}",
                        CareContexts = new List<CareContextRepresentation>
                        {
                            new CareContextRepresentation(
                                $"{patientFromTmh.Identifier}",
                                $"{patientFromTmh.FirstName}  {patientFromTmh.LastName}")
                        },
                        PhoneNumber = patientFromTmh.PhoneNumber,
                        YearOfBirth = (ushort) patientFromTmh.DateOfBirth.Year
                    };
                    try
                    {
                        patient.Gender = Enum.Parse<Gender>(patientFromTmh.Gender);
                    }
                    catch (ArgumentException e)
                    {
                        Log.Error(e.Message);
                        patient.Gender = Gender.M;
                    }

                    patients.Add(patient);
                }

                return patients.AsQueryable();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }
    }
}