namespace In.ProjectEKA.HipService.Discovery
{
    using System;
    using System.Linq;
    using HipLibrary.Patient.Model;

    public static class DiscoveryUseCase
    {
        public static ValueTuple<PatientEnquiryRepresentation, ErrorRepresentation> DiscoverPatient(
            IQueryable<PatientEnquiryRepresentation> patients)
        {
            if (!patients.Any())
            {
                return new ValueTuple<PatientEnquiryRepresentation, ErrorRepresentation>(null,
                    new ErrorRepresentation(new Error(ErrorCode.NoPatientFound, "No patient found")));
            }

            if (patients.Count() <= 1)
                return new ValueTuple<PatientEnquiryRepresentation, ErrorRepresentation>(patients.First(), null);
            var errorResponse =
                new ErrorRepresentation(new Error(ErrorCode.MultiplePatientsFound, "Multiple patients found"));
            return new ValueTuple<PatientEnquiryRepresentation, ErrorRepresentation>(null, errorResponse);
        }
    }
}