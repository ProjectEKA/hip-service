namespace In.ProjectEKA.HipLibrary.Patient
{
    using Model;
    using Optional;

    public interface IPatientRepository
    {
        Option<Patient> PatientWith(string referenceNumber);
    }
}