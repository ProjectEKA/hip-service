namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model.Response;
    
    public class DataFlow : IDataFlow
    {
        private readonly IDataFlowRepository dataFlowRepository;

        public DataFlow(IDataFlowRepository dataFlowRepository)
        {
            this.dataFlowRepository = dataFlowRepository;
        }
        
        public async Task<Tuple<HealthInformationResponse, ErrorResponse>> HealthInformationRequestFor(HealthInformationRequest request)
        {
            if (!IsValidConsentArtefact(request.Consent))
            {
                return new Tuple<HealthInformationResponse, ErrorResponse>(null, null);
            }
            
            var result = await dataFlowRepository.SaveRequestFor(request.TransactionId, request)
                .ConfigureAwait(false);
            return result.Map(r =>
            {
                var errorResponse = new ErrorResponse(new Error(ErrorCode.ServerInternalError,
                    ErrorMessage.InternalServerError));
                return new Tuple<HealthInformationResponse, ErrorResponse>(null, errorResponse);
            }).ValueOr(new Tuple<HealthInformationResponse,
                ErrorResponse>(new HealthInformationResponse(request.TransactionId), null));
        }

        private static bool IsValidConsentArtefact(Consent consent)
        {
            return true;
        }
    }
}