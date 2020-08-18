using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using In.ProjectEKA.HipLibrary.Patient;
using In.ProjectEKA.HipLibrary.Patient.Model;
using In.ProjectEKA.HipService.Logger;

namespace In.ProjectEKA.HipService.OpenMrs
{
    public class OpenMrsCareContextRepository : ICareContextRepository
    {
        private readonly IOpenMrsClient openMrsClient;
        public OpenMrsCareContextRepository(IOpenMrsClient openMrsClient)
        {
            this.openMrsClient = openMrsClient;
        }

        public async Task<IEnumerable<CareContextRepresentation>> GetCareContexts(string patientReferenceNumber)
        {
            var combinedCareContexts = new List<CareContextRepresentation>();
            var programCareContexts = await LoadProgramEnrollments(patientReferenceNumber);
            combinedCareContexts.AddRange(programCareContexts);

            var visitCareContexts = await LoadVisits(patientReferenceNumber);
            combinedCareContexts.AddRange(visitCareContexts);

            return combinedCareContexts;
        }

        public virtual async Task<List<CareContextRepresentation>> LoadProgramEnrollments(string uuid)
        {
            var path = DiscoveryPathConstants.OnProgramEnrollmentPath;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrEmpty(uuid))
            {
                query["patient"] = uuid;
                query["v"] = "full";
            }
            if (query.ToString() != "")
            {
                path = $"{path}/?{query}";
            }

            var response = await openMrsClient.GetAsync(path);
            var content = await response.Content.ReadAsStringAsync();

            var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;

            var careContexts = new List<CareContextRepresentation>();
            var results = root.GetProperty("results");
            for (int i = 0; i < results.GetArrayLength(); i++)
            {
                var attributes = TryGetProperty(results[i], "attributes");
                if (attributes.GetArrayLength() == 0) {
                    LogAndThrowException($"Property 'attributes' is empty when getting program enrollments.");
                }
                var referenceNumber = TryGetProperty(attributes[0], "value");
                var display = TryGetProperty(results[i], "display");
                careContexts.Add(new CareContextRepresentation(referenceNumber.GetString(), display.GetString()));
            }

            return careContexts;
        }

        private JsonElement TryGetProperty(JsonElement data, string propertyName) {
            if (!data.TryGetProperty(propertyName, out var property)) {
                LogAndThrowException($"Property '{propertyName}' is missing when getting program enrollments.");
            }
            return property;
        }

        private void LogAndThrowException(string message) {
            Log.Error(message);
            throw new OpenMrsFormatException();
        }

        public virtual async Task<List<CareContextRepresentation>> LoadVisits(string uuid)
        {
            var path = DiscoveryPathConstants.OnVisitPath;
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrEmpty(uuid))
            {
                query["patient"] = uuid;
                query["v"] = "full";
            }
            if (query.ToString() != "")
            {
                path = $"{path}/?{query}";
            }

            var response = await openMrsClient.GetAsync(path);
            var content = await response.Content.ReadAsStringAsync();

            var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;

            var careContexts = new List<CareContextRepresentation>();
            var results = root.GetProperty("results");
            for (int i = 0; i < results.GetArrayLength(); i++)
            {
                var visitType = results[i].GetProperty("visitType");
                var display = visitType.GetProperty("display").GetString();
                careContexts.Add(new CareContextRepresentation(null, display));
            }

            List<CareContextRepresentation> uniqueCareContexts = careContexts
                .GroupBy(careContext => careContext.Display)
                .Select(visitType => visitType.First())
                .ToList();

            return uniqueCareContexts;
        }
    }
}