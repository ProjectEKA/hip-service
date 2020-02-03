using Optional;

namespace In.ProjectEKA.DefaultHip.Link
{
    using HipLibrary.Patient.Model;

    public interface IPatientRepository
    {
        Option<Patient> PatientWith(string referenceNumber);
    }
}