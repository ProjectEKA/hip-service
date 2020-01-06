using System.Collections.Generic;
using System.Linq;
using hip_service.Discovery.Patient.Helpers;
using hip_service.Discovery.Patient.models;

namespace hip_service.Link.Patient
{
    public class PatientRepository
    {
        private readonly string _filePath;

        public PatientRepository(string filePath)
        {
            _filePath = filePath;
        }

        
        IEnumerable<Discovery.Patient.Model.Patient> GetAllPatientFromJson()
        {
            var patientsInfo = FileReader.ReadJson(_filePath);
            return patientsInfo;
        }

        public Discovery.Patient.Model.Patient GetPatientInfoWithReferenceNumber(string referenceNumber)
        {
            var patientsInfo = GetAllPatientFromJson();
            return patientsInfo.First(patient => patient.Identifier == referenceNumber);
        }

        public CareContext GetProgramInfo(string patientReferenceNumber, string programReferenceNumber)
        {
            var patientInfo = GetPatientInfoWithReferenceNumber(patientReferenceNumber);
            return patientInfo.CareContexts.First(program => program.ReferenceNumber == programReferenceNumber);
        }
        
    }
}