namespace In.ProjectEKA.HipService.Link.Model
{
    public class Acknowledgement
    {
        public AcknowledgementStatus Status { get; }
        
        public Acknowledgement(AcknowledgementStatus status)
        {
            Status = status;
        }
    }
}