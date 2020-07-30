namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using In.ProjectEKA.HipLibrary.Validation;

    [AtLeastOneHasValueValidation("Name", "Gender", ErrorMessage = "Patient name or gender must be provided.")]
    public class PatientEnquiry
    {
        /// <summary>
        /// Identifier of patient at consent manager
        /// </summary>
        [Required(ErrorMessage = "Patient id must be provided.")]
        public string Id { get; }

        public IEnumerable<Identifier> VerifiedIdentifiers { get; }

        public IEnumerable<Identifier> UnverifiedIdentifiers { get; }

        public string Name { get; }

        public Gender? Gender { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        // TODO: Need to be used while doing discovery
        public ushort? YearOfBirth { get; }

        public PatientEnquiry(
            string id,
            IEnumerable<Identifier> verifiedIdentifiers,
            IEnumerable<Identifier> unverifiedIdentifiers,
            string name,
            Gender? gender,
            ushort? yearOfBirth)
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