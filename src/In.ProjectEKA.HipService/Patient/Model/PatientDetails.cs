namespace In.ProjectEKA.HipService.Patient.Model
{
    using System.Collections.Generic;
    using HipLibrary.Patient.Model;

    public class PatientDetails
    {
        public string Name { get; }
        public Gender Gender { get; }
        public string HealthId { get; }
        public string Address { get; }
        public string State { get; }
        public string District { get; }
        public uint YearOfBirth { get; }
        public uint DayOfBirth { get; }
        public uint MonthOfBirth { get; }
        public List<Identifier> VerifiedIdentifiers { get; }
        public List<Identifier> UnverifiedIdentifiers { get; }

        public PatientDetails(string name,
                              Gender gender,
                              string healthId,
                              string address,
                              string state,
                              string district,
                              uint yearOfBirth,
                              uint dayOfBirth,
                              uint monthOfBirth,
                              List<Identifier> verifiedIdentifiers,
                              List<Identifier> unverifiedIdentifiers
                              )
        {
            Name = name;
            Gender = gender;
            HealthId = healthId;
            Address = address;
            State = state;
            District = district;
            YearOfBirth = yearOfBirth;
            DayOfBirth = dayOfBirth;
            MonthOfBirth = monthOfBirth;
            VerifiedIdentifiers = verifiedIdentifiers;
            UnverifiedIdentifiers = unverifiedIdentifiers;
        }
    }
}