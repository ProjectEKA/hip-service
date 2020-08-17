namespace In.ProjectEKA.HipService.Link.Model
{
    using System;

    public class AddContextsAcknowledgement
    {
        public string Status { get; }
        public AddContextsAcknowledgement(string status)
        {
            Status = status;
        }
    }
}