using System;
using System.Linq;
using hip_library.Patient.models;

namespace hip_service.Discovery.Patient
{
    using System.Collections.Generic;

    public class DiscoveryUseCase
    {
        public Tuple<hip_library.Patient.models.Patient, Error> DiscoverPatient(
            IEnumerable<hip_library.Patient.models.Patient> patients)
        {
            if (!patients.Any())
            {
                return new Tuple<hip_library.Patient.models.Patient, Error>(null,
                    new Error(ErrorCode.NoPatientFound, "No patient found"));
            }

            if (patients.Count() > 1)
            {
                return new Tuple<hip_library.Patient.models.Patient, Error>(null,
                    new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found"));
            }

            return new Tuple<hip_library.Patient.models.Patient, Error>(patients.First(), null);
        }
    }
}