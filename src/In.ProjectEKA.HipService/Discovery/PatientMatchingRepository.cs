namespace In.ProjectEKA.HipService.Discovery
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Helper;
    using Model;
    using DiscoveryRequest = HipLibrary.Patient.Model.Request.DiscoveryRequest;
    using static Matcher.StrongMatcherFactory;

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