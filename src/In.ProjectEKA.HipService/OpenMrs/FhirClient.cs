using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using In.ProjectEKA.HipService.Logger;

namespace In.ProjectEKA.HipService.OpenMrs
{
    public class FhirClient: IOpenMrsClient
    {
        private readonly HttpClient httpClient;
        private readonly OpenMrsConfiguration configuration;

        public FhirClient(HttpClient httpClient, OpenMrsConfiguration openMrsConfiguration)
        {
            this.httpClient = httpClient;
            configuration = openMrsConfiguration;

            SettingUpHeaderAuthorization();
        }

        public async Task<HttpResponseMessage> GetAsync(string openMrsUrl)
        {
            try {
                return await httpClient.GetAsync(Path.Join(configuration.Url, openMrsUrl));
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
                throw exception;
            }
        }

        private void SettingUpHeaderAuthorization()
        {
            var authToken = Encoding.ASCII.GetBytes($"{configuration.Username}:{configuration.Password}");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(authToken));
        }
    }
}