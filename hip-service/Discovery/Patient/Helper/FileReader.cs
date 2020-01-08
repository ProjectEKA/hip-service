namespace hip_service.Discovery.Patient.Helper
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public static class FileReader
    {
        public static IEnumerable<Model.Patient> ReadJson(string patientFilePath)
        {
            var jsonData = File.ReadAllText(patientFilePath);
            return JsonConvert.DeserializeObject<List<Model.Patient>>(jsonData);
        }

        public static async Task<IEnumerable<Model.Patient>> ReadJsonAsync(string patientFilePath)
        {
            var jsonData = await File.ReadAllTextAsync(patientFilePath);
            return JsonConvert.DeserializeObject<List<Model.Patient>>(jsonData);
        }
    }
}