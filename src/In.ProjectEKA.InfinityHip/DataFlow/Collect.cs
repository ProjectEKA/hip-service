namespace In.ProjectEKA.DefaultHip.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using HipLibrary.Patient;
    using HipLibrary.Patient.Model;
    using Hl7.Fhir.Model;
    using Newtonsoft.Json;
    using Optional;
    using Patient;
    using Serilog;

    public class Collect : ICollect
    {
        public async Task<Option<Entries>> CollectData(DataRequest dataRequest)
        {
            var bundles = new List<Bundle>();
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
            var formatStrings = new[]
            {
                "yyyy-MM-dd", "yyyy-MM-dd hh:mm:ss", "yyyy-MM-dd hh:mm:ss tt", "yyyy-MM-ddTHH:mm:ss.fffzzz",
                "dd/MM/yyyy", "dd/MM/yyyy hh:mm:ss", "dd/MM/yyyy hh:mm:ss tt", "dd/MM/yyyyTHH:mm:ss.fffzzz"
            };
            DateTime.TryParseExact(dateString, formatStrings, CultureInfo.CurrentCulture, DateTimeStyles.None,
                out var aDateTime);
            return aDateTime;
        }

        private IEnumerable<string> FindPatientData(DataRequest request)
        {
            try
            {
                LogDataRequest(request);
                var jsonData = File.ReadAllText("demoPatientCareContextDataMap.json");
                var patientDataMap =
                    JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<PatientCcRecord>>>>(
                        jsonData);

                var listOfDataFiles = new List<string>();
                foreach (var grantedContext in request.CareContexts)
                {
                    var refData = patientDataMap[grantedContext.PatientReference];
                    var ccData = refData?[grantedContext.CareContextReference];
                    ccData?
                        .Select(ccRecord => new {ccRecord, captureTime = ParseDate(ccRecord.CapturedOn)})
                        .Where(t => WithinRange(request.DataRange, t.captureTime))
                        .SelectMany(t => request.HiType, (t, hiType) => new {t, hiType})
                        .Select(t => new {t, hiTypeStr = t.hiType.ToString().ToLower()})
                        .Select(t => t.t.t.ccRecord.Data.GetValueOrDefault(t.hiTypeStr) ?? new List<string>())
                        .Where(dataFiles => dataFiles.Count > 0)
                        .Aggregate(listOfDataFiles, (source, next) =>
                        {
                            source.AddRange(next);
                            return source;
                        });
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
            Log.Information(
                "Data request received." +
                $" transactionId:{request.TransactionId} , " +
                $"CareContexts:{ccList}, " +
                $"HiTypes:{requestedHiTypes}," +
                $" From date:{request.DataRange.From}," +
                $" To date:{request.DataRange.To}, " +
                $"CallbackUrl:{request.CallBackUrl}");
        }
    }
}