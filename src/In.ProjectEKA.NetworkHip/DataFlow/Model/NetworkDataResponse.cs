namespace In.ProjectEKA.DefaultHip.DataFlow.Model
{
    using System.Collections.Generic;

    public class NetworkDataResponse
    {
        public IEnumerable<NetworkData> Results { get; set; }

        public NetworkDataResponse(IEnumerable<NetworkData> results)
        {
            Results = results;
        }
    }
}