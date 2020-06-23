using System;
using Serilog;

namespace In.ProjectEKA.TMHHip.Discovery
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model;
    using Newtonsoft.Json;
    using HipLibrary.Matcher;
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
                return result.Select(patient => new HipLibrary.Patient.Model.Patient
                {
                    Name = $"{patient.FirstName} {patient.LastName}",
                    Gender = Enum.Parse<Gender>(patient.Gender),
                    Identifier = patient.Identifier,
                    CareContexts = new List<CareContextRepresentation>
                    {
                        new CareContextRepresentation(
                            $"{patient.Identifier}",
                            $"{patient.FirstName}  {patient.LastName}")
                    },
                    PhoneNumber = patient.PhoneNumber,
                    YearOfBirth = (ushort) patient.DateOfBirth.Year
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