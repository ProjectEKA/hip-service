using In.ProjectEKA.DefaultHip.Discovery.Model;
using Optional;

namespace In.ProjectEKA.DefaultHip.Link
{
    public interface IPatientRepository
    {
        Option<Patient> PatientWith(string referenceNumber);
    }
}