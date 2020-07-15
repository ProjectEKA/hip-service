using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    public class OpenMrsClient: IOpenMrsClient
    {
        private readonly HttpClient httpClient;
        private readonly OpenMrsConfiguration configuration;

        public OpenMrsClient(HttpClient httpClient, OpenMrsConfiguration openmrsConfiguration)
        {
            this.httpClient = httpClient;
            configuration = openmrsConfiguration;

            SettingUpHeaderAuthorization();
        }

        public async Task<HttpResponseMessage> GetAsync(string openmrsUrl)
        {
            return await httpClient.GetAsync(Path.Join(configuration.Url, openmrsUrl));
        }

        private void SettingUpHeaderAuthorization()
        {
            var authToken = Encoding.ASCII.GetBytes($"{configuration.Username}:{configuration.Password}");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(authToken));
        }
    }
}