
using System.Collections.Generic;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using In.ProjectEKA.HipLibrary.Patient.Model;
using Patient = Hl7.Fhir.Model.Patient;

namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    public class DiscoveryDataSource : IPatientDal
    {
        private readonly IOpenMrsClient openMrsClient;

        public DiscoveryDataSource(IOpenMrsClient openMrsClient)
        {
            this.openMrsClient = openMrsClient;
        }

        public async Task<List<Patient>> LoadPatientsAsync(string name, Gender? gender, string yearOfBirth)
        {
            var patients = new List<Patient>();
            var response = await openMrsClient.GetAsync(DiscoveryPathConstants.OnPatientPath);
            var content = await response.Content.ReadAsStringAsync();
            var bundle = new FhirJsonParser().Parse<Bundle>(content);
            bundle.Entry.ForEach(entry => {patients.Add((Patient) entry.Resource);});
            return patients;
        }
    }
}