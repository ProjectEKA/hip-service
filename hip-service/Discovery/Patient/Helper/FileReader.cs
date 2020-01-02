using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using hip_service.Discovery.Patient.models;
using Newtonsoft.Json;

namespace hip_service.Discovery.Patient.Helpers
{
    public static class FileReader
    {
        public static IEnumerable<models.Patient> ReadJson(string patientFilePath)
        {
            var jsonData = File.ReadAllText(patientFilePath);
            return JsonConvert.DeserializeObject<List<models.Patient>>(jsonData);
        }

        public static async Task<IEnumerable<models.Patient>> ReadJsonAsync(string patientFilePath)
        {
            var jsonData = await File.ReadAllTextAsync(patientFilePath);
            return JsonConvert.DeserializeObject<List<models.Patient>>(jsonData);
        }
    }
}