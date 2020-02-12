using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace In.ProjectEKA.DefaultHip.Patient
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

        public static async Task<T> ReadJsonAsync<T>(string filePath) where T : Base
        {
            var jsonData = await File.ReadAllTextAsync(filePath);
            FhirJsonParser fjp = new FhirJsonParser();
            return fjp.Parse<T>(jsonData);
        }

        public static T ReadJson<T>(string filePath)
        {
            var jsonData = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(jsonData);
        }
    }
}