namespace In.ProjectEKA.TMHHip.Discovery
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using Patient = HipLibrary.Patient.Model.Patient;

    public class PatientMatchingRepository : IMatchingRepository
    {
        private readonly HttpClient client;

        public PatientMatchingRepository(HttpClient client)
        {
            this.client = client;
        }

        public Task<IQueryable<Patient>> Where(DiscoveryRequest request)
        {
              return Task.FromResult(new List<Patient>
                        {
                            new Patient
                            {
                                Identifier = "5",
                                PhoneNumber = "8340289040",
                                CareContexts = new List<CareContextRepresentation>
                                {
                                    new CareContextRepresentation("131", "National Cancer Program")
                                },
                                FirstName = "Ron",
                                LastName = "Doe"
                            }
                        }.AsQueryable());
        }
    }
}