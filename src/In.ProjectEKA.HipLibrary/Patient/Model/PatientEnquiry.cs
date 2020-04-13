namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System;
    using System.Collections.Generic;

    public class PatientEnquiry
    {
        public string Id { get; }

        public IEnumerable<Identifier> VerifiedIdentifiers { get; }

        public IEnumerable<Identifier> UnverifiedIdentifiers { get; }

        public string Name { get; }

        public Gender Gender { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        // TODO: Need to be used while doing discovery
        public int? YearOfBirth { get; }

        public PatientEnquiry(
            string id,
            IEnumerable<Identifier> verifiedIdentifiers,
            IEnumerable<Identifier> unverifiedIdentifiers,
            string name,
            Gender gender,
            int? yearOfBirth)
        {
            Id = id;
            VerifiedIdentifiers = verifiedIdentifiers;
            UnverifiedIdentifiers = unverifiedIdentifiers;
            Name = name;
            Gender = gender;
            YearOfBirth = yearOfBirth;
        }
    }
}