namespace In.ProjectEKA.HipService.Common.Model
{
    using System;
    using System.Collections.Generic;

    public class ConsentArtefact
    {
        public ConsentArtefact(string schemaVersion,
            string consentId,
            DateTime createdAt,
            ConsentPurpose purpose,
            PatientReference patient,
            HIPReference hip, IEnumerable<HiType> hiTypes, ConsentPermission permission,
            IEnumerable<GrantedContext> careContexts,
            OrganizationReference consentManager)
        {
            ConsentId = consentId;
            CreatedAt = createdAt;
            Purpose = purpose;
            Patient = patient;
            Hip = hip;
            HiTypes = hiTypes;
            Permission = permission;
            CareContexts = careContexts;
            ConsentManager = consentManager;
        }

        public string SchemaVersion { get; }
        public string ConsentId { get; }
        public DateTime CreatedAt { get; }
        public ConsentPurpose Purpose { get; }
        public PatientReference Patient { get; }
        public HIPReference Hip { get; }
        public OrganizationReference ConsentManager { get; }
        public IEnumerable<HiType> HiTypes { get; }
        public ConsentPermission Permission { get; }
        public IEnumerable<GrantedContext> CareContexts { get; }
    }
}