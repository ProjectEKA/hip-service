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
    using Hl7.Fhir.Utility;
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
            var bundles = new List<CareBundle>();
            var patientData = FindPatientData(dataRequest);
            var careContextReferences = patientData.Keys.ToList();
            foreach (var careContextReference in careContextReferences)
                foreach (var result in patientData.GetOrDefault(careContextReference))
                {
                    Log.Information($"Returning file: {result}");
                    bundles.Add(new CareBundle(careContextReference, await FileReader.ReadJsonAsync<Bundle>(result)));
                }

            var entries = new Entries(bundles);
            return Option.Some(entries);
        }

        private static bool WithinRange(DateRange range, DateTime date)
        {
            var fromDate = ParseDate(range.From).AddHours(2);
            var toDate = ParseDate(range.To);
            return date > fromDate && date < toDate;
        }

        private static DateTime ParseDate(string dateString)
        {
            var formatStrings = new[]
            {
                "yyyy-MM-dd", "yyyy-MM-dd hh:mm:ss", "yyyy-MM-dd hh:mm:ss tt", "yyyy-MM-ddTHH:mm:ss.fffzzz",
                "yyyy-MM-dd'T'HH:mm:ss.fff", "yyyy-MM-dd'T'HH:mm:ss.ffff", "yyyy-MM-dd'T'HH:mm:ss.fffff",
                "yyyy-MM-dd'T'HH:mm:ss", "yyyy-MM-dd'T'HH:mm:ss.ff", "yyyy-MM-dd'T'HH:mm:ss.ff",
                "dd/MM/yyyy", "dd/MM/yyyy hh:mm:ss", "dd/MM/yyyy hh:mm:ss tt", "dd/MM/yyyyTHH:mm:ss.fffzzz",
                "yyyy-MM-dd'T'HH:mm:ss.ffffff"
            };
            var tryParseExact = DateTime.TryParseExact(dateString,
                formatStrings,
                CultureInfo.CurrentCulture,
                DateTimeStyles.None,
                out var aDateTime);
            if (!tryParseExact)
                Log.Error($"Error parsing date: {dateString}");

            return aDateTime;
        }

        private Dictionary<string, List<string>> FindPatientData(DataRequest request)
        {
            try
            {
                LogDataRequest(request);
                var jsonData = File.ReadAllText(careContextMapFile);
                var patientDataMap = JsonConvert
                    .DeserializeObject<Dictionary<string, Dictionary<string, List<CareContextRecord>>>>(jsonData);

                var careContextsAndListOfDataFiles = new Dictionary<string, List<string>>();

                foreach (var grantedContext in request.CareContexts)
                {
                    var refData = patientDataMap[grantedContext.PatientReference];
                    var ccData = refData?[grantedContext.CareContextReference];
                    var listOfDataFiles = new List<string>();
                    if (ccData == null) continue;
                    // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                    foreach (var ccRecord in ccData)
                    {
                        var captureTime = ParseDate(ccRecord.CapturedOn);
                        if (!WithinRange(request.DateRange, captureTime)) continue;
                        foreach (var hiType in request.HiType)
                        {
                            var hiTypeStr = hiType.ToString().ToLower();
                            var dataFiles = ccRecord.Data.GetValueOrDefault(hiTypeStr) ?? new List<string>();
                            if (dataFiles.Count > 0)
                                listOfDataFiles.AddRange(dataFiles);
                        }
                    }
                    careContextsAndListOfDataFiles.Add(grantedContext.CareContextReference, listOfDataFiles);
                }

                return careContextsAndListOfDataFiles;
            }
            catch (Exception e)
            {
                Log.Error("Error Occured while collecting data. {Error}", e);
            }

            return new Dictionary<string, List<string>>();
        }

        private static void LogDataRequest(DataRequest request)
        {
            var ccList = JsonConvert.SerializeObject(request.CareContexts);
            var requestedHiTypes = string.Join(", ", request.HiType.Select(hiType => hiType.ToString()));
            Log.Information("Data request received." +
                            $" transactionId:{request.TransactionId} , " +
                            $"CareContexts:{ccList}, " +
                            $"HiTypes:{requestedHiTypes}," +
                            $" From date:{request.DateRange.From}," +
                            $" To date:{request.DateRange.To}, " +
                            $"CallbackUrl:{request.DataPushUrl}");
        }
    }
}