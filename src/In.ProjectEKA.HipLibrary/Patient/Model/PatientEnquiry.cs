namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using System.Xml.Serialization;

    public class PatientEnquiry
    {
        [JsonPropertyName("id")]
        [XmlElement("id")]
        public string Id { get; }

        [JsonPropertyName("verifiedIdentifiers")]
        [XmlElement("verifiedIdentifiers")]
        public IEnumerable<Identifier> VerifiedIdentifiers { get; }

        [JsonPropertyName("unverifiedIdentifiers")]
        [XmlElement("unverifiedIdentifiers")]
        public IEnumerable<Identifier> UnverifiedIdentifiers { get; }

        [JsonPropertyName("firstName")]
        [XmlElement("firstName")]
        public string FirstName { get; }

        [JsonPropertyName("lastName")]
        [XmlElement("lastName")]
        public string LastName { get; }

        [JsonPropertyName("gender")]
        [XmlElement("gender")]
        public Gender Gender { get; }

        [JsonPropertyName("dateOfBirth")]
        [XmlElement("dateOfBirth")]
        public DateTime DateOfBirth { get; }

        public PatientEnquiry(
            string id, 
            IEnumerable<Identifier> verifiedIdentifiers,
            IEnumerable<Identifier> unverifiedIdentifiers, 
            string firstName, 
            string lastName, 
            Gender gender,
            DateTime dateOfBirth)
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