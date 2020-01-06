using System;
using System.Linq;
using HipLibrary.Patient.Models.Response;

namespace hip_service.Discovery.Patient
{
    public static class DiscoveryUseCase
    {
        public static Tuple<HipLibrary.Patient.Models.Response.Patient, Error> DiscoverPatient(
            IQueryable<HipLibrary.Patient.Models.Response.Patient> patients)
        {
            if (!patients.Any())
            {
                return new Tuple<HipLibrary.Patient.Models.Response.Patient, Error>(null,
                    new Error(ErrorCode.NoPatientFound, "No patient found"));
            }

            if (patients.Count() > 1)
            {
                return new Tuple<HipLibrary.Patient.Models.Response.Patient, Error>(null,
                    new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found"));
            }

            return new Tuple<HipLibrary.Patient.Models.Response.Patient, Error>(patients.First(), null);
        }
    }
}