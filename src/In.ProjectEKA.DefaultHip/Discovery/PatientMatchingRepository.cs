namespace In.ProjectEKA.DefaultHip.Discovery
{
    using System.Linq;
    using System.Threading.Tasks;
    using HipLibrary.Matcher;
    using HipLibrary.Patient.Model;
    using Patient;
    using static HipLibrary.Matcher.StrongMatcherFactory;

    public class PatientMatchingRepository : IMatchingRepository
    {
        private readonly string patientFilePath;

        public PatientMatchingRepository(string patientFilePath)
        {
            this.patientFilePath = patientFilePath;
        }

        public async Task<IQueryable<Patient>> Where(DiscoveryRequest request)
        {
            var expression = GetVerifiedExpression(request.Patient.VerifiedIdentifiers);
            var patientsInfo = await FileReader.ReadJsonAsync(patientFilePath);
            return patientsInfo.Where(expression.Compile()).AsQueryable();
        }
    }
}