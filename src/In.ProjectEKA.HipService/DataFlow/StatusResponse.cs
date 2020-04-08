namespace In.ProjectEKA.HipService.DataFlow
{
    public class StatusResponse
    {

        public string CareContextReference { get; }
        public HiStatus HiStatus { get; }
        public string Description { get; }

        public StatusResponse(string careContextReference, HiStatus hiStatus, string description)
        {
            CareContextReference = careContextReference;
            HiStatus = hiStatus;
            Description = description;
        }
    }
}