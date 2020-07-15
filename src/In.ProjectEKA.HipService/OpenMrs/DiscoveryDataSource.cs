
using System.Collections.Generic;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    public class DiscoveryDataSource : IPatientDal
    {
        private readonly IOpenMrsClient openMrsClient;

        public DiscoveryDataSource(IOpenMrsClient openMrsClient)
        {
            this.openMrsClient = openMrsClient;
        }

        public async System.Threading.Tasks.Task<List<Hl7.Fhir.Model.Patient>> LoadPatientsAsync(string name, Gender? gender, string yearOfBirth)
        {
            var response = await openMrsClient.GetAsync("path/to/resource");
            var content = await response.Content.ReadAsStringAsync();
            var parser = new FhirJsonParser();
            var bundle = parser.Parse<Bundle>(content);
            return new List<Hl7.Fhir.Model.Patient>{ (Hl7.Fhir.Model.Patient) bundle.Entry[0].Resource};
        }
    }
}