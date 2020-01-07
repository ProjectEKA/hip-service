using System;
using System.Linq;
using HipLibrary.Patient.Models.Response;

namespace hip_service.Discovery.Patient
{
    public static class DiscoveryUseCase
    {
        public static Tuple<HipLibrary.Patient.Models.Response.Patient, ErrorResponse> DiscoverPatient(
            IQueryable<HipLibrary.Patient.Models.Response.Patient> patients)
        {
            if (!patients.Any())
            {
                var errorResponse = new ErrorResponse(new Error(ErrorCode.NoPatientFound, "No patient found"));
                return new Tuple<HipLibrary.Patient.Models.Response.Patient, ErrorResponse>(null, errorResponse);
            }

            if (patients.Count() > 1)
            {
                var errorResponse =
                    new ErrorResponse(new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found"));
                return new Tuple<HipLibrary.Patient.Models.Response.Patient, ErrorResponse>(null, errorResponse);
            }

            return new Tuple<HipLibrary.Patient.Models.Response.Patient, ErrorResponse>(patients.First(), null);
        }
    }
}