using System;
using System.Globalization;
using System.Linq;
using Hl7.FhirPath.Sprache;

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
            var fromDate = ParseDate(range.From);
            var toDate = ParseDate(range.To);
            return date >= fromDate && date < toDate;
        }

        private static DateTime ParseDate(string dateString)
        {
            var formatStrings = new string[]
            {
                "yyyy-MM-dd", "yyyy-MM-dd hh:mm:ss", "yyyy-MM-dd hh:mm:ss tt", "yyyy-MM-ddTHH:mm:ss.fffzzz", 
                "dd/MM/yyyy", "dd/MM/yyyy hh:mm:ss", "dd/MM/yyyy hh:mm:ss tt", "dd/MM/yyyyTHH:mm:ss.fffzzz" 
            };
            DateTime.TryParseExact(dateString, formatStrings, CultureInfo.CurrentCulture, DateTimeStyles.None,
                out var aDateTime);
            return aDateTime;
        }

        private List<string> FindPatientData(DataRequest request)
        {
            try
            {
                LogDataRequest(request);
                var jsonData = File.ReadAllText("demoPatientCareContextDataMap.json");
                var patientDataMap =
                    JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<PatientCCRecord>>>>(
                        jsonData);

                var listOfDataFiles = new List<string>();
                foreach (var grantedContext in request.CareContexts)
                {
                    var refData = patientDataMap[grantedContext.PatientReference];
                    var ccData = refData?[grantedContext.CareContextReference];
                    if (ccData == null) continue;
                    foreach (var ccRecord in ccData)
                    {
                        var captureTime = ParseDate(ccRecord.capturedOn);
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
                Log.Error("Error Occured while collecting data. {Error}", e);
            }

            return new List<string>();
        }

        private static void LogDataRequest(DataRequest request)
        {
            var ccList = JsonConvert.SerializeObject(request.CareContexts);
            var requestedHiTypes = string.Join(", ", request.HiType.Select(hiType => hiType.ToString()));
            var transactionId = request.TransactionId;
            var from = request.DataRange.From;
            var to = request.DataRange.To;
            var callbackUrl = request.CallBackUrl;
            Log.Information(
                $"Data request received. transactionId:{transactionId} , CareContexts:{ccList}, HiTypes:{requestedHiTypes}, From date:{from}, To date:{to}, CallbackUrl:{callbackUrl}",
                transactionId, ccList, requestedHiTypes, from,to, callbackUrl);
        }
    }
}