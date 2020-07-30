namespace In.ProjectEKA.HipService.OpenMrs
{
    using System.Linq;
    using System;
    using System.Threading.Tasks;
    using HipLibrary.Matcher;
    using HipLibrary.Patient.Model;
    using DiscoveryRequest = HipLibrary.Patient.Model.DiscoveryRequest;
    using In.ProjectEKA.HipService.OpenMrs.Mappings;

    public class OpenMrsPatientMatchingRepository : IMatchingRepository
    {
        private readonly IPatientDal _patientDal;
        public OpenMrsPatientMatchingRepository(IPatientDal patientDal)
        {
            _patientDal = patientDal;
        }

        public async Task<IQueryable<Patient>> Where(DiscoveryRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var result =
                await _patientDal.LoadPatientsAsync(
                    request.Patient?.Name,
                    request.Patient?.Gender.ToOpenMrsGender(),
                    request.Patient?.YearOfBirth?.ToString());

            return result
                    .Select(p => p.ToHipPatient(request.Patient?.Name))
                    .ToList()
                    .AsQueryable();
        }
    }
}