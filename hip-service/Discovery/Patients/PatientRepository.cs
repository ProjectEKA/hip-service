using System.Collections.Generic;
using System.Linq;
using hip_library.Patient.models.domain;
using hip_service.Discovery.Patients.Helpers;

namespace hip_service.Discovery.Patients
{
    public class PatientRepository : IPatientRepository
    {
        private readonly string patientFilePath;

        public PatientRepository(string patientFilePath)
        {
            this.patientFilePath = patientFilePath;
        }

        public IEnumerable<Patient> GetPatients(string phoneNumber, string caseId, string firstName, string lastName)
        {
            var patients = FileReader.ReadJson(patientFilePath);

            return patients
                .Where(patient => patient.PhoneNumber == phoneNumber ||
                                  patient.FirstName == firstName ||
                                  patient.LastName == lastName || patient.CaseId == caseId)
                .Select(patient => new Patient(
                    "patient",
                    patient.FirstName + " " + patient.LastName,
                    patient.Gender,
                    patient.BirthDate,
                    new Address("home", patient.Address.Line, patient.Address.City, patient.Address.District,
                        patient.Address.State, patient.Address.PostalCode),
                    new Contact(patient.Contact.Name,
                        new ContactPoint("home", "https://tmc.gov.in/ncg/telecom", patient.Contact.PhoneNumber))));
        }
    }
}