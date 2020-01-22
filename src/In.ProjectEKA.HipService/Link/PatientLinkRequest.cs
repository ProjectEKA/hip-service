namespace In.ProjectEKA.HipService.Link
{
    public class PatientLinkRequest
    {
        public PatientLinkRequest(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }
}