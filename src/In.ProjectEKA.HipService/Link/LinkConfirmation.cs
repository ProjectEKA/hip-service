namespace In.ProjectEKA.HipService.Link
{
    public class LinkConfirmation
    {
        public string LinkRefNumber { get; }
        public string Token { get; }
        
        public LinkConfirmation(string linkRefNumber, string token)
        {
            LinkRefNumber = linkRefNumber;
            Token = token;
        }
    }
}