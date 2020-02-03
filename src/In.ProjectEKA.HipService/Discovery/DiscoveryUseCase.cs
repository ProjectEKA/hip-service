namespace In.ProjectEKA.HipService.Discovery
{
    using System;
    using System.Linq;
    using HipLibrary.Patient.Model.Response;

    public static class DiscoveryUseCase
    {
        public static Tuple<Patient, ErrorResponse> DiscoverPatient(IQueryable<Patient> patients)
        {
            if (!patients.Any())
            {
                return new Tuple<Patient, ErrorResponse>(null,
                    new ErrorResponse(new Error(ErrorCode.NoPatientFound, "No patient found")));
            }

            if (patients.Count() <= 1) return new Tuple<Patient, ErrorResponse>(patients.First(), null);
            var errorResponse =
                new ErrorResponse(new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found"));
            return new Tuple<Patient, ErrorResponse>(null, errorResponse);
        }
    }
}