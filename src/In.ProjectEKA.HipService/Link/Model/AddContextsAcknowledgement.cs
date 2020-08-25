namespace In.ProjectEKA.HipService.Link.Model
{
    public class AddContextsAcknowledgement
    {
        public AddContextsAcknowledgement(string status)
        {
            Status = status;
        }

        public string Status { get; }
    }
}