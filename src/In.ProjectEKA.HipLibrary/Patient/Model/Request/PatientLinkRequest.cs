namespace In.ProjectEKA.HipLibrary.Patient.Model.Request
{ 
    public class PatientLinkRequest
    { 
        public string Token { get; }
        public string LinkReferenceNumber { get; }
        public PatientLinkRequest(string token, string linkReferenceNumber)
        {
            Token = token;
            LinkReferenceNumber = linkReferenceNumber;
        }
    }
}