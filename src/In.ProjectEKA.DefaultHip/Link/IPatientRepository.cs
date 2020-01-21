using In.ProjectEKA.DefaultHip.Discovery.Model;
using Optional;

namespace In.ProjectEKA.DefaultHip.Link
{
    public interface IPatientRepository
    {
        Option<Patient> PatientWith(string referenceNumber);

        Option<CareContext> ProgramInfoWith(string patientReferenceNumber, string programReferenceNumber);
    }
}