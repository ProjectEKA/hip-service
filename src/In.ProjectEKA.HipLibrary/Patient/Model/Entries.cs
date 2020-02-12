using System.Collections.Generic;
using Hl7.Fhir.Model;
using Optional;

namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class Entries
    {
        public Entries(IEnumerable<Bundle> bundles)
        {
            Bundles = bundles;
        }

        public IEnumerable<Bundle> Bundles { get; }
    }
}