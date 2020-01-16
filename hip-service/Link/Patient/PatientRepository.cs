using System;
using System.Collections.Generic;
using System.Linq;
using hip_service.Discovery.Patient.Helper;
using hip_service.Discovery.Patient.Model;
using Optional;

namespace hip_service.Link.Patient
{
    public class PatientRepository: IPatientRepository
    {
        private readonly string filePath;

        public PatientRepository(string filePath)
        {
            this.filePath = filePath;
        }
        
        public IEnumerable<Discovery.Patient.Model.Patient> All()
        {
            var patientsInfo = FileReader.ReadJson(filePath);
            return patientsInfo;
        }

        public Option<Discovery.Patient.Model.Patient> PatientWith(string referenceNumber)
        {
            try
            {
                var patientsInfo = All();
                var patient = patientsInfo.First(p => p.Identifier == referenceNumber);
                return Option.Some(patient); 
            }
            catch (Exception)
            {
                return Option.None<Discovery.Patient.Model.Patient>();
            }
        }

        public Option<CareContext> ProgramInfoWith(string patientReferenceNumber, string programReferenceNumber)
        {
            try
            {
                return PatientWith(patientReferenceNumber)
                    .Map(patient => patient.CareContexts.First(program =>
                        program.ReferenceNumber == programReferenceNumber));
            }
            catch (Exception)
            {
                return Option.None<CareContext>();
            }
        }
    }
}