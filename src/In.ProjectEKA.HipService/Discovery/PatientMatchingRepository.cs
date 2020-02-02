namespace In.ProjectEKA.HipService.Discovery
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Helper;
    using Model;

    public class PatientMatchingRepository : IMatchingRepository
    {
        private readonly string patientFilePath;

        public PatientMatchingRepository(string patientFilePath)
        {
            this.patientFilePath = patientFilePath;
        }

        public async Task<IQueryable<Patient>> Where(Expression<Func<Patient, bool>> predicate)
        {
            var patientsInfo = await FileReader.ReadJsonAsync(patientFilePath);
            return patientsInfo.Where(predicate.Compile()).AsQueryable();
        }
    }
}