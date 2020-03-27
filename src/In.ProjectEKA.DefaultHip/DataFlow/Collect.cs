using System;
using Optional.Collections;

namespace In.ProjectEKA.DefaultHip.DataFlow
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Hl7.Fhir.Model;
    using In.ProjectEKA.DefaultHip.Patient;
    using In.ProjectEKA.HipLibrary.Patient;
    using In.ProjectEKA.HipLibrary.Patient.Model;
    using Newtonsoft.Json;
    using Optional;
    using Serilog;

    public class Collect : ICollect
    {
        private readonly HiTypeDataMap hiTypeDataMap;

        public Collect(HiTypeDataMap hiTypeDataMap)
        {
            this.hiTypeDataMap = hiTypeDataMap;
        }

        public async Task<Option<Entries>> CollectData(DataRequest dataRequest)
        {
            var bundles = new List<Bundle> { };
            var results = FindPatientData(dataRequest);
            foreach (var item in results)
            {
                Log.Information($"Returning file: {item}");
                bundles.Add(await FileReader.ReadJsonAsync<Bundle>(item));
            }
            var entries = new Entries(bundles);
            return Option.Some(entries);
        }

        private bool WithinRange(HiDataRange range, DateTime date)
        {
            var fromDate = DateTime.ParseExact(range.From, "yyyy-MM-dd", null);
            var toDate = DateTime.ParseExact(range.To, "yyyy-MM-dd", null);
            return date >= fromDate && date < toDate;
        }

        public List<string> FindPatientData(DataRequest request) {
            try
            {
                var jsonData = File.ReadAllText("demoPatientCareContextDataMap.json");
                var patientDataMap = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<PatientCCRecord>>>>(jsonData);

                var listOfDataFiles = new List<string>();
                foreach (var grantedContext in request.CareContexts)
                {
                    var refData = patientDataMap[grantedContext.PatientReferenceNumber];
                    var ccData = refData?[grantedContext.CareContextReferenceNumber];
                    if (ccData == null) continue;
                    foreach (var ccRecord in ccData)
                    {
                        var captureTime = DateTime.ParseExact(ccRecord.capturedOn, "yyyy-MM-dd", null);
                        if (!WithinRange(request.DataRange, captureTime)) continue;
                        foreach (var hiType in request.HiType)
                        {
                            var hiTypeStr = hiType.ToString().ToLower();
                            var dataFiles = ccRecord.data.GetValueOrDefault(hiTypeStr) ?? new List<string>();
                            if (dataFiles.Count > 0)
                            {
                                listOfDataFiles.AddRange(dataFiles);
                            }
                        }
                    }
                }
                return listOfDataFiles;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return new List<string>();
        }
    }
}