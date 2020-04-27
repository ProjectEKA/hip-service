namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Collections.Generic;
    using Hl7.Fhir.Model;

    public class Entries
    {
        public Entries(Dictionary<string, List<Bundle>> bundles)
        {
            Bundles = bundles;
        }
        
        public Dictionary<string,List<Bundle>> Bundles { get; }
    }
}