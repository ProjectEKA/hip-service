namespace In.ProjectEKA.HipService.Link
{
    using System;

    [Obsolete]
    public class PatientLinkRequest
    {
        public PatientLinkRequest(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }
}