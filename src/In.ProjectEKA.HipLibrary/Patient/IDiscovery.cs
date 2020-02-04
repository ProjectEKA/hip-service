namespace In.ProjectEKA.HipLibrary.Patient
{
    using System;
    using System.Threading.Tasks;
    using Model;

    public interface IDiscovery
    {
        Task<Tuple<DiscoveryRepresentation, ErrorRepresentation>> PatientFor(DiscoveryRequest request);
    }
}