namespace In.ProjectEKA.HipService.Link
{
    public class LinkConfirmation
    {
        public LinkConfirmation(string linkRefNumber, string token)
        {
            LinkRefNumber = linkRefNumber;
            Token = token;
        }

        public string LinkRefNumber { get; }
        public string Token { get; }
    }
}