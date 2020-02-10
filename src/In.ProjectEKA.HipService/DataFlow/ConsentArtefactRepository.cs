namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Collections.Generic;

    public class ConsentArtefactRepository : IConsentArtefactRepository
    {
        public Tuple<ConsentArtefact, Exception> GetFor(string consentId)
        {
            var consentPurpose = new ConsentPurpose("Encounter", "ENCOUNTER","Some");
            var patientReference = new PatientReference("25@ncg");
            var hipReference = new HIPReference("10000005", "Xyz centre" );
            var hiTypes = new List<HiType> {HiType.Condition};
            var consentPermission = new ConsentPermission(
                AccessMode.View,
                new AccessPeriod(DateTime.Now, DateTime.Today),
                new DataFrequency(DataFrequencyUnit.Day, 1, 1),
                DateTime.Today);
            var grantedContexts = new List<GrantedContext> {new GrantedContext("12345",
                "batman@tmh")};   
            
            var consentArtefact = new ConsentArtefact(consentId,
                DateTime.Now,
                consentPurpose,
                patientReference,
                hipReference,
                hiTypes,
                consentPermission,
                grantedContexts); 

            return new Tuple<ConsentArtefact, Exception>(consentArtefact, null);
        }
    }
}