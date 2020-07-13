using System;
using System.Net.Http;

namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    public class OpenMrsClient
    {
        private readonly HttpClient httpClient;
        private readonly OpenMrsConfiguration configuration;

        public OpenMrsClient(HttpClient httpClient, OpenMrsConfiguration openmrsConfiguration)
        {
            this.httpClient = httpClient;
            configuration = openmrsConfiguration;
        }

        public object Get(string openmrsUrl)
        {
            throw new NotImplementedException();
        }
    }
}