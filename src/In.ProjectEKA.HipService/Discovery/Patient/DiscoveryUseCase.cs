namespace In.ProjectEKA.HipService.Discovery.Patient
{
    using System;
    using System.Linq;
    using HipLibrary.Patient.Model.Response;

    public static class DiscoveryUseCase
    {
        public static Tuple<Patient, ErrorResponse> DiscoverPatient(
            IQueryable<Patient> patients)
        {
            if (!patients.Any())
            {
                var errorResponse = new ErrorResponse(new Error(ErrorCode.NoPatientFound, "No patient found"));
                return new Tuple<Patient, ErrorResponse>(null, errorResponse);
            }

            if (patients.Count() > 1)
            {
                var errorResponse =
                    new ErrorResponse(new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found"));
                return new Tuple<Patient, ErrorResponse>(null, errorResponse);
            }

            return new Tuple<Patient, ErrorResponse>(patients.First(), null);
        }
    }
}