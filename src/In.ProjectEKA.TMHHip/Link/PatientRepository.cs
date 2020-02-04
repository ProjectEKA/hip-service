namespace In.ProjectEKA.TMHHip.Link
{
    using System.Collections.Generic;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using Optional;

    public class PatientRepository : IPatientRepository
    {
        public Option<Patient> PatientWith(string referenceNumber)
        {
            return Option.Some(new Patient
            {
                Identifier = "5",
                PhoneNumber = "8340289040",
                CareContexts = new List<CareContextRepresentation>
                {
                    new CareContextRepresentation("131", "National Cancer Program")
                },
                FirstName = "Ron",
                LastName = "Doe"
            });
        }
    }
}