using System.Collections;
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

        IEnumerable<PatientInfo> GetAllPatientFromJson()
        {
            var patientsInfo = FileReader.ReadJson(_filePath);
            return patientsInfo;
        }

        public PatientInfo GetPatientInfoWithReferenceNumber(string referenceNumber)
        {
            var patientsInfo = GetAllPatientFromJson();
            return patientsInfo.First(patient => patient.Identifier == referenceNumber);
        }

        public ProgramInfo GetProgramInfo(string patientReferenceNumber, string programReferenceNumber)
        {
            var patientInfo = GetPatientInfoWithReferenceNumber(patientReferenceNumber);
            return patientInfo.Programs.First(program => program.ReferenceNumber == programReferenceNumber);
        }
        
    }
}