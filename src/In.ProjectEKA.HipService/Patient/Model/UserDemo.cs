using System.Collections.Generic;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipService.Patient.Model
{
    public class UserDemographics
    {
        public string Name { get; }
        public Gender Gender { get; }
        public string HealthId { get; }
        public Address Address { get; }
        public uint YearOfBirth { get; }
        public uint DayOfBirth { get; }
        public uint MonthOfBirth { get; }
        public List<Identifier> Identifiers { get; }

        public UserDemographics(string name,
            Gender gender,
            string healthId,
            Address address,
            uint yearOfBirth,
            uint dayOfBirth,
            uint monthOfBirth,
            List<Identifier> identifiers
        )
        {
            Name = name;
            Gender = gender;
            HealthId = healthId;
            Address = address;
            YearOfBirth = yearOfBirth;
            DayOfBirth = dayOfBirth;
            MonthOfBirth = monthOfBirth;
            Identifiers = identifiers;
        }
    }
}
