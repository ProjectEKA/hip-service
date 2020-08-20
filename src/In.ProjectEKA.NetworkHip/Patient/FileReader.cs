namespace In.ProjectEKA.DefaultHip.Patient
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Hl7.Fhir.Model;
    using Hl7.Fhir.Serialization;
    using Newtonsoft.Json;
    using Patient = HipLibrary.Patient.Model.Patient;

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
            var fjp = new FhirJsonParser();
            return fjp.Parse<T>(jsonData);
        }
    }
}