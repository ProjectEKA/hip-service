using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace hip_service.Discovery.Patient
{
    using Helper;

    public class PatientMatchingRepository : IMatchingRepository
    {
        private readonly string patientFilePath;

        public PatientMatchingRepository(string patientFilePath)
        {
            this.patientFilePath = patientFilePath;
        }

        public async Task<IQueryable<Model.Patient>> Where(Expression<Func<Model.Patient, bool>> predicate)
        {
            var patientsInfo = await FileReader.ReadJsonAsync(patientFilePath);
            return patientsInfo.Where(predicate.Compile()).AsQueryable();
        }
    }
}