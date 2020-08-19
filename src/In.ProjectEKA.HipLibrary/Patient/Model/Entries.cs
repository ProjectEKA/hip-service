namespace In.ProjectEKA.HipLibrary.Patient.Model
{
    using System.Collections.Generic;

    public class Entries
    {
        public Entries(IEnumerable<CareBundle> careBundles)
        {
            CareBundles = careBundles;
        }

        public IEnumerable<CareBundle> CareBundles { get; }
    }
}