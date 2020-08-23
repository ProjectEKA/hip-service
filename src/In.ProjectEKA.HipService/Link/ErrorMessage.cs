namespace In.ProjectEKA.HipService.Link
{
    public static class ErrorMessage
    {
        public static readonly string NoLinkRequestFound = "No request found";
        public static readonly string CareContextNotFound = "Care Context Not Found";
        public static readonly string NoPatientFound = "No patient Found";
        public static readonly string DatabaseStorageError = "Unable to store data to Database";
        public static readonly string DiscoveryRequestNotFound = "Discovery request does not exist.";
        public static readonly string OtpServiceError = "Otp service not working";
        public static readonly string DuplicateRequestId = "Request id is not unique";
        public static readonly string RequestIdExists = "Discovery Request already exists";
        public static readonly string FailedToGetLinkedCareContexts = "Failed to get Linked Care Contexts";
        public static readonly string HipConnection = "HIP connection error";
        public static readonly string HipConfiguration = "HIP configuration error. If you encounter this issue repeatedly, please report it";
    }
}