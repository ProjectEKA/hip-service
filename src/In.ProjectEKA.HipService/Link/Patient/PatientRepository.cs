namespace In.ProjectEKA.HipService.Link.Patient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DefaultHip.Discovery.Helper;
    using DefaultHip.Discovery.Model;
    using Optional;

    public class PatientRepository : IPatientRepository
    {
        private readonly string filePath;

        public PatientRepository(string filePath)
        {
            this.filePath = filePath;
        }

        public Option<Patient> PatientWith(string referenceNumber)
        {
            try
            {
                var patientsInfo = All();
                var patient = patientsInfo.First(p => p.Identifier == referenceNumber);
                return Option.Some(patient);
            }
            catch (Exception)
            {
                return Option.None<Patient>();
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

        public IEnumerable<Patient> All()
        {
            var patientsInfo = FileReader.ReadJson(filePath);
            return patientsInfo;
        }
    }
}