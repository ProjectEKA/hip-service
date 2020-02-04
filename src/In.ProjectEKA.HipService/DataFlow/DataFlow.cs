namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model;

    public class DataFlow : IDataFlow
    {
        private readonly IDataFlowRepository dataFlowRepository;

        public DataFlow(IDataFlowRepository dataFlowRepository)
        {
            this.dataFlowRepository = dataFlowRepository;
        }
        
        public async Task<Tuple<HealthInformationResponse, ErrorRepresentation>> HealthInformationRequestFor(HealthInformationRequest request)
        {
            if (!IsValidConsentArtefact(request.Consent))
            {
                return new Tuple<HealthInformationResponse, ErrorRepresentation>(null, null);
            }
            
            var result = await dataFlowRepository.SaveRequestFor(request.TransactionId, request)
                .ConfigureAwait(false);
            return result.Map(r =>
            {
                var errorResponse = new ErrorRepresentation(new Error(ErrorCode.ServerInternalError,
                    ErrorMessage.InternalServerError));
                return new Tuple<HealthInformationResponse, ErrorRepresentation>(null, errorResponse);
            }).ValueOr(new Tuple<HealthInformationResponse,
                ErrorRepresentation>(new HealthInformationResponse(request.TransactionId), null));
        }

        private static bool IsValidConsentArtefact(Consent consent)
        {
            return true;
        }
    }
}