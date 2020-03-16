namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System;
    using System.Collections.Generic;

    public class PatientEnquiry
    {
        public string Id { get; }

        public IEnumerable<Identifier> VerifiedIdentifiers { get; }

        public IEnumerable<Identifier> UnverifiedIdentifiers { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public Gender Gender { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        // TODO: Need to be used while doing discovery
        public DateTime? DateOfBirth { get; }

        public PatientEnquiry(
            string id,
            IEnumerable<Identifier> verifiedIdentifiers,
            IEnumerable<Identifier> unverifiedIdentifiers,
            string firstName,
            string lastName,
            Gender gender,
            DateTime? dateOfBirth)
        {
            Id = id;
            VerifiedIdentifiers = verifiedIdentifiers;
            UnverifiedIdentifiers = unverifiedIdentifiers;
            FirstName = firstName;
            LastName = lastName;
            Gender = gender;
            DateOfBirth = dateOfBirth;
        }
    }
}