namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Collections.Generic;
    using Hl7.Fhir.Model;

    public class Entries
    {
        public Entries(IEnumerable<CareBundle> careBundles)
        {
            CareBundles = careBundles;
        }
        
        public IEnumerable<CareBundle> CareBundles { get; }
    }
}