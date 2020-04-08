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
        private readonly string careContextMapFile;

        public Collect(string careContextMapFile)
        {
            this.careContextMapFile = careContextMapFile;
        }

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

        private static bool WithinRange(HiDataRange range, DateTime date)
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
            var tryParseExact = DateTime.TryParseExact(dateString,
                formatStrings,
                CultureInfo.CurrentCulture,
                DateTimeStyles.None,
                out var aDateTime);
            if (!tryParseExact)
            {
                Log.Error($"Error parsing date: {dateString}");
            }

            return aDateTime;
        }

        private IEnumerable<string> FindPatientData(DataRequest request)
        {
            try
            {
                LogDataRequest(request);
                var jsonData = File.ReadAllText(careContextMapFile);
                var patientDataMap = JsonConvert
                    .DeserializeObject<Dictionary<string, Dictionary<string, List<CareContextRecord>>>>(jsonData);

                var listOfDataFiles = new List<string>();
                foreach (var grantedContext in request.CareContexts)
                {
                    var refData = patientDataMap[grantedContext.PatientReference];
                    var ccData = refData?[grantedContext.CareContextReference];
                    if (ccData == null) continue;
                    // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                    foreach (var ccRecord in ccData)
                    {
                        var captureTime = ParseDate(ccRecord.CapturedOn);
                        if (!WithinRange(request.DataRange, captureTime)) continue;
                        foreach (var hiType in request.HiType)
                        {
                            var hiTypeStr = hiType.ToString().ToLower();
                            var dataFiles = ccRecord.Data.GetValueOrDefault(hiTypeStr) ?? new List<string>();
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
            Log.Information("Data request received." +
                            $" transactionId:{request.TransactionId} , " +
                            $"CareContexts:{ccList}, " +
                            $"HiTypes:{requestedHiTypes}," +
                            $" From date:{request.DataRange.From}," +
                            $" To date:{request.DataRange.To}, " +
                            $"CallbackUrl:{request.CallBackUrl}");
        }
    }
}