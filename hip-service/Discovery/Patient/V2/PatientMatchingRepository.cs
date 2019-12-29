namespace hip_service.Discovery.Patient
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using hip_service.Discovery.Patient.models;
    using System.Threading.Tasks;
    using hip_service.Discovery.Patient.Helpers;

    public class PatientMatchingRepository : IMatchingRepository
    {
        private readonly string patientFilePath;

        public PatientMatchingRepository(String patientFilePath)
        {
            this.patientFilePath = patientFilePath;
        }

        public async Task<IQueryable<PatientInfo>> Where(Expression<Func<PatientInfo, bool>> predicate)
        {
            var patientsInfo = await FileReader.ReadJsonAsync(patientFilePath);

            return patientsInfo.Where(predicate.Compile()).AsQueryable();
        }
    }
}