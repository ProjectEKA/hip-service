namespace In.ProjectEKA.HipLibrary.Patient
{
    using System.Threading.Tasks;
    using Model;
    using Optional;

    public interface IPatientRepository
    {
        Task<Option<Patient>> PatientWith(string referenceNumber);
    }
}