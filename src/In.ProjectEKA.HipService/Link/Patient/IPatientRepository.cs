namespace In.ProjectEKA.HipService.Link.Patient
{
    using Discovery.Patient.Model;
    using Optional;

    public interface IPatientRepository
    {
        Option<Patient> PatientWith(string referenceNumber);

        Option<CareContext> ProgramInfoWith(string patientReferenceNumber, string programReferenceNumber);
    }
}