using Hl7.Fhir.Model;

namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    public class CareBundle
    {
        public CareBundle(string careContextReference, Bundle bundleForThisCcr)
        {
            CareContextReference = careContextReference;
            BundleForThisCcr = bundleForThisCcr;
        }

        public string CareContextReference { get; }

        public Bundle BundleForThisCcr { get; }
    }
}