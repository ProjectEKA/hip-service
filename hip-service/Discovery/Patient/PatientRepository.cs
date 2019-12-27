using System.Linq;
using System.Threading.Tasks;
using hip_library.Patient.models;
using hip_service.Discovery.Patient.Helpers;

namespace hip_service.Discovery.Patient
{
    public class PatientRepository : IPatientRepository
    {
        private readonly string patientFilePath;

        public PatientRepository(string patientFilePath)
        {
            this.patientFilePath = patientFilePath;
        }

        Task<IQueryable<hip_library.Patient.models.Patient>> IPatientRepository.SearchPatients(string phoneNumber,
            string caseReferenceNumber, string firstName,
            string lastName)
        {
            var patientsInfo = FileReader.ReadJson(patientFilePath);

            var patients = patientsInfo
                .Where(patient => patient.PhoneNumber == phoneNumber)
                .Where(patient => firstName == null || patient.FirstName == firstName)
                .Where(patient => lastName == null || patient.LastName == lastName)
                .Select(patient =>
                {
                    var careContexts = patient.Programs
                        .Where(program =>
                            caseReferenceNumber == null || program.ReferenceNumber == caseReferenceNumber)
                        .Select(program => new CareContextRepresentation(
                            program.ReferenceNumber,
                            program.Description))
                        .ToList();

                    return new hip_library.Patient.models.Patient(
                        patient.Identifier,
                        patient.FirstName + " " + patient.LastName,
                        careContexts, new[] { "" });
                });

            return Task.FromResult(patients.AsQueryable());
        }

    }
}