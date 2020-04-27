namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Collections.Generic;
    using Hl7.Fhir.Model;

    public class Entries
    {
        public Entries(List<CareBundle> careBundles)
        {
            CareBundles = careBundles;
        }
        
        public List<CareBundle> CareBundles { get; }
    }
}