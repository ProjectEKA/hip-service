namespace In.ProjectEKA.DefaultHip.DataFlow
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Hl7.Fhir.Model;
    using In.ProjectEKA.DefaultHip.Patient;
    using In.ProjectEKA.HipLibrary.Patient;
    using In.ProjectEKA.HipLibrary.Patient.Model;
    using Optional;

    public class Collect : ICollect
    {
        private readonly HiTypeDataMap hiTypeDataMap;

        public Collect(HiTypeDataMap hiTypeDataMap)
        {
            this.hiTypeDataMap = hiTypeDataMap;
        }

        public async Task<Option<Entries>> CollectData(DataRequest dataRequest)
        {
            var bundles = new List<Bundle> {};
            var map = hiTypeDataMap.GetMap();
            foreach (HiType hiType in dataRequest.HiType)
            {
                var dataList = map.GetValueOrDefault(hiType) ?? new List<string>();
                foreach (string item in dataList) {
                    bundles.Add(await FileReader.ReadJsonAsync<Bundle>(item));
                }
            }
            var entries = new Entries(bundles);
            return Option.Some(entries);
        }
    }
}