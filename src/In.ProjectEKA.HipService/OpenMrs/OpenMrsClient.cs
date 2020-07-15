using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using In.ProjectEKA.HipLibrary.Patient.Model;

namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    public class OpenMrsClient: IPatientDal
    {
        private readonly HttpClient httpClient;
        private readonly OpenMrsConfiguration configuration;

        public OpenMrsClient(HttpClient httpClient, OpenMrsConfiguration openmrsConfiguration)
        {
            this.httpClient = httpClient;
            configuration = openmrsConfiguration;
        }

        public async Task<HttpResponseMessage> GetAsync(string openmrsUrl)
        {
            var authToken = Encoding.ASCII.GetBytes($"{configuration.Username}:{configuration.Password}");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(authToken));

            return await httpClient.GetAsync(configuration.Url + openmrsUrl);;
        }

        public List<Hl7.Fhir.Model.Patient> LoadPatients(string name, Gender? gender, string yearOfBirth)
        {
            throw new NotImplementedException();
        }
    }
}