using System.Collections.Generic;
using System.IO;
using hip_service.Models.dto;
using Newtonsoft.Json;

namespace hip_service.Discovery.Patients.Helpers
{
    public static class FileReader
    {
        public static IEnumerable<PatientInfo> ReadJson(string patientFilePath)
        {
            var jsonData = File.ReadAllText(patientFilePath);
            return JsonConvert.DeserializeObject<List<PatientInfo>>(jsonData);
        }
    }
}