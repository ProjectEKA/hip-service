namespace In.ProjectEKA.DefaultHip.Link
{
    using System.Collections.Generic;
    using System.IO;
    using Model;
    using Newtonsoft.Json;

    public static class FileReader
    {
        public static IEnumerable<Patient> ReadJson(string patientFilePath)
        {
            var jsonData = File.ReadAllText(patientFilePath);
            return JsonConvert.DeserializeObject<List<Patient>>(jsonData);
        }
    }
}