namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Model;
    using Database;
    using Logger;
    using Model;
    using Optional;

    public class DataFlowRepository : IDataFlowRepository
    {
        private readonly DataFlowContext dataFlowContext;

        public DataFlowRepository(DataFlowContext dataFlowContext)
        {
            this.dataFlowContext = dataFlowContext;
        }

        public async Task<Option<Exception>> SaveRequestFor(
            string transactionId,
            HealthInformationRequest request)
        {
            var dataFlowRequest = new DataFlowRequest(transactionId, request);
            try
            {
                dataFlowContext.DataFlowRequest.Add(dataFlowRequest);
                await dataFlowContext.SaveChangesAsync();
                return Option.None<Exception>();
            }
            catch (Exception exception)
            {
                Log.Fatal(exception,"Error Occured");
                return Option.Some(exception);
            }
        }
        
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