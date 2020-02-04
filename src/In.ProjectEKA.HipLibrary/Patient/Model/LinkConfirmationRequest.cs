namespace In.ProjectEKA.HipLibrary.Patient.Model
{ 
    public class LinkConfirmationRequest
    { 
        public string Token { get; }
        
        public string LinkReferenceNumber { get; }
        public LinkConfirmationRequest(string token, string linkReferenceNumber)
        {
            Token = token;
            LinkReferenceNumber = linkReferenceNumber;
        }
    }
}