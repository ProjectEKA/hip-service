namespace In.ProjectEKA.HipService.Discovery
{
    using System;
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model;

    public interface IDiscovery
    {
        Task<Tuple<DiscoveryRepresentation, ErrorRepresentation>> PatientFor(DiscoveryRequest request);
    }
}