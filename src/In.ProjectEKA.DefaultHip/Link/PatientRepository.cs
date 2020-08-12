namespace In.ProjectEKA.DefaultHip.Link
{
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using Patient;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Optional;
    using System.Threading.Tasks;

    public class PatientRepository : IPatientRepository
    {
        private readonly string filePath;

        public PatientRepository(string filePath)
        {
            this.filePath = filePath;
        }

        public async Task<Option<Patient>> PatientWithAsync(string referenceNumber)
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

        private IEnumerable<Patient> All()
        {
            var patientsInfo = FileReader.ReadJson(filePath);
            return patientsInfo;
        }
    }
}