namespace In.ProjectEKA.DefaultHip.Discovery
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using HipLibrary.Patient.Model;
    using Newtonsoft.Json;

    public static class FileReader
    {
        public static async Task<IEnumerable<Patient>> ReadJsonAsync(string patientFilePath)
        {
            var jsonData = await File.ReadAllTextAsync(patientFilePath);
            return JsonConvert.DeserializeObject<List<Patient>>(jsonData);
        }
        
        public static IEnumerable<Patient> ReadJson(string patientFilePath)
        {
            var jsonData = File.ReadAllText(patientFilePath);
            return JsonConvert.DeserializeObject<List<Patient>>(jsonData);
        }
    }
}