using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using hip_service.Discovery.Patient.Helpers;

namespace hip_service.Discovery.Patient
{
    public class PatientMatchingRepository : IMatchingRepository
    {
        private readonly string patientFilePath;

        public PatientMatchingRepository(string patientFilePath)
        {
            this.patientFilePath = patientFilePath;
        }

        public async Task<IQueryable<models.Patient>> Where(Expression<Func<models.Patient, bool>> predicate)
        {
            var patientsInfo = await FileReader.ReadJsonAsync(patientFilePath);
            return patientsInfo.Where(predicate.Compile()).AsQueryable();
        }
    }
}