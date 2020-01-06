#nullable enable
using System;
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

        public Discovery.Patient.Model.Patient? GetPatientInfoWithReferenceNumber(string referenceNumber)
        {
            try
            {
                var patientsInfo = GetAllPatientFromJson();
                var patient = patientsInfo.First(patient => patient.Identifier == referenceNumber);
                return patient;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public CareContext GetProgramInfo(string patientReferenceNumber, string programReferenceNumber)
        {
            try
            {
                var patientInfo = GetPatientInfoWithReferenceNumber(patientReferenceNumber);
                var careContext = patientInfo.CareContexts.First(program => program.ReferenceNumber == programReferenceNumber);
                return careContext;
            }
            catch (Exception e)
            { 
                return null;
            }
            

        }
        
    }
}