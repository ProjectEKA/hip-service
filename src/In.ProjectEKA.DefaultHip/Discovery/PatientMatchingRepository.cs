namespace In.ProjectEKA.DefaultHip.Discovery
{
    using System.Linq;
    using System.Threading.Tasks;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using Patient;
    using DiscoveryRequest = HipLibrary.Patient.Model.Request.DiscoveryRequest;
    using static StrongMatcherFactory;

    public class PatientMatchingRepository : IMatchingRepository
    {
        private readonly string patientFilePath;

        public PatientMatchingRepository(string patientFilePath)
        {
            this.patientFilePath = patientFilePath;
        }
        public async Task<IQueryable<Patient>> Where(DiscoveryRequest request)
        {
            var expression = GetExpression(request.Patient.VerifiedIdentifiers);
            var patientsInfo = await FileReader.ReadJsonAsync(patientFilePath);
            return patientsInfo.Where(expression.Compile()).AsQueryable();
        }
    }
}