namespace In.ProjectEKA.HipServiceTest.Common.Builder
{
    using System;
    using System.Collections.Generic;
    using HipService.Common.Model;

    internal class ConsentArtefactBuilder
    {
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
            return new ConsentArtefact(ConsentId, CreatedAt, Purpose, Patient, Hip, HiTypes, Permission, CareContexts, ConsentManager);
        }

        
    }
}