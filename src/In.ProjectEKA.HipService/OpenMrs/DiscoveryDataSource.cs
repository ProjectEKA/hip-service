
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
            var response = await openMrsClient.GetAsync("ws/fhir2/Patient");
            var content = await response.Content.ReadAsStringAsync();
            var parser = new FhirJsonParser();
            var bundle = parser.Parse<Bundle>(content);
            var patients = new List<Hl7.Fhir.Model.Patient>();
            bundle.Entry.ForEach(entry => {patients.Add((Hl7.Fhir.Model.Patient) entry.Resource);});
            return patients;
        }
    }
}