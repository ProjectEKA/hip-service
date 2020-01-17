namespace In.ProjectEKA.HipService.Discovery.Patient.Helper
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Model;
    using Newtonsoft.Json;

    public static class FileReader
    {
        public static IEnumerable<Patient> ReadJson(string patientFilePath)
        {
            var jsonData = File.ReadAllText(patientFilePath);
            return JsonConvert.DeserializeObject<List<Patient>>(jsonData);
        }

        public static async Task<IEnumerable<Patient>> ReadJsonAsync(string patientFilePath)
        {
            var jsonData = await File.ReadAllTextAsync(patientFilePath);
            return JsonConvert.DeserializeObject<List<Patient>>(jsonData);
        }
    }
}