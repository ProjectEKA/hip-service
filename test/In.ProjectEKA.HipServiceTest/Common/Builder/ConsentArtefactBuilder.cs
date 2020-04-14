namespace In.ProjectEKA.HipServiceTest.Common.Builder
{
    using System;
    using System.Collections.Generic;
    using HipService.Common.Model;

    internal class ConsentArtefactBuilder
    {
        public string SchemaVersion; 
        public string ConsentId; 
        public DateTime CreatedAt;
        public ConsentPurpose Purpose;
        public PatientReference Patient;
        public HIPReference Hip;
        public IEnumerable<HiType> HiTypes;
        public ConsentPermission Permission;
        public IEnumerable<GrantedContext> CareContexts;
        public OrganizationReference ConsentManager;
        
        public ConsentArtefact Build()
        {
            return new ConsentArtefact(SchemaVersion, ConsentId, CreatedAt, Purpose, Patient, Hip, HiTypes, Permission, CareContexts, ConsentManager);
        }

        
    }
}