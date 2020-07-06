using System.Linq;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipService.Common
{
    public static class MaskingUtility
    {
        public static PatientEnquiryRepresentation GetMaskedPatientEnquiryRepresentation(
            PatientEnquiryRepresentation patient)
        {
            patient.ReferenceNumber = new string(patient.ReferenceNumber.ToCharArray().Reverse().ToArray());
            foreach (var careContextRepresentation in patient.CareContexts.AsEnumerable())
            {
                careContextRepresentation.ReferenceNumber =
                    new string(careContextRepresentation.ReferenceNumber.ToCharArray().Reverse().ToArray());
            }

            return patient;
        }

        public static string GetMaskedPatientReference(string patientReference)
        {
            return new string(patientReference.ToCharArray().Reverse().ToArray());
        }

        public static LinkEnquiry GetUnmaskedLinkEnquiry(LinkEnquiry linkEnquiry)
        {
            foreach (var careContextEnquiry in linkEnquiry.CareContexts)
            {
                careContextEnquiry.ReferenceNumber =
                    new string(careContextEnquiry.ReferenceNumber.ToCharArray().Reverse().ToArray());
            }

            return linkEnquiry;
        }
    }
}