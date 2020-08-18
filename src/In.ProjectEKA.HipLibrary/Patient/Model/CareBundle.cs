namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using Hl7.Fhir.Model;

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