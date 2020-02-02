using Optional;

namespace In.ProjectEKA.DefaultHip.Link
{
    using Model;

    public interface IPatientRepository
    {
        Option<Patient> PatientWith(string referenceNumber);
    }
}