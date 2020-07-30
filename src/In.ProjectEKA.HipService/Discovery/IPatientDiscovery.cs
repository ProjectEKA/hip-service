using System;
using System.Threading.Tasks;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipService.Discovery
{
    public interface IPatientDiscovery
    {
        Task<ValueTuple<DiscoveryRepresentation, ErrorRepresentation>> PatientFor(
            DiscoveryRequest request);
    }
}