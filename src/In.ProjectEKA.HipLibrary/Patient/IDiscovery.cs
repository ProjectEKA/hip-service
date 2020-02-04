namespace In.ProjectEKA.HipLibrary.Patient
{
    using System;
    using System.Threading.Tasks;
    using Model.Request;
    using Model.Response;

    public interface IDiscovery
    {
        Task<Tuple<DiscoveryResponse, ErrorResponse>> PatientFor(DiscoveryRequest request);
    }
}