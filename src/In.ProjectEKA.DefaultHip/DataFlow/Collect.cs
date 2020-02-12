using System.Collections.Generic;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using In.ProjectEKA.DefaultHip.Patient;
using In.ProjectEKA.HipLibrary.Patient;
using In.ProjectEKA.HipLibrary.Patient.Model;
using Optional;

namespace In.ProjectEKA.DefaultHip.DataFlow
{
    public class Collect : ICollect
    {
        private readonly string filePath;

        public Collect(string filePath)
        {
            this.filePath = filePath;
        }
        public async Task<Option<Entries>> CollectData(DataRequest dataRequest)
        {
            var bundle = await FileReader.ReadJsonAsync<Bundle>(filePath);
            var bundles = new List<Bundle> {bundle};
            var entries = new Entries(bundles);
            return Option.Some(entries);
        }
    }
}