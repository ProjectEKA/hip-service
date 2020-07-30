using System.Collections.Generic;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipServiceTest.Discovery.Builder
{
    public struct User
    {
        public string Name { get; private set; }
        public string Id { get; private set; }
        public Gender Gender { get; private set; }
        public ushort YearOfBirth { get; private set; }
        public string PhoneNumber { get; private set; }
        public IEnumerable<CareContextRepresentation> CareContexts { get; set; }

        public static readonly User Krunal = new User
        {
            Name = "Krunal Patel",
            Id = "RVH1111",
            Gender = Gender.M,
            YearOfBirth = 1976,
            CareContexts = new List<CareContextRepresentation> {
                new CareContextRepresentation("NCP1111", "National Cancer program"),
                new CareContextRepresentation("NCP1111", "National Cancer program - Episode 2")
            }
        };
        public static readonly User JohnDoe = new User
        {
            Name = "John Doe",
            Id = "1234",
            Gender = Gender.M,
            YearOfBirth = 1994,
            CareContexts = null,
            PhoneNumber = "11111111111"
        };
    }
}