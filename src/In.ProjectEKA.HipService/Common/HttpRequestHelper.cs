namespace In.ProjectEKA.HipService.Common
{
    using System;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Text;
    using Microsoft.Net.Http.Headers;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public static class HttpRequestHelper
    {
        public static HttpRequestMessage CreateHttpRequest<T>(
            string url,
            T content,
            string token,
            string cmSuffix)
        {
            var json = JsonConvert.SerializeObject(content, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri($"{url}"))
            {
                Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json)
            };
            if (token != null)
                httpRequestMessage.Headers.Add(HeaderNames.Authorization, token);
            if (cmSuffix != null)
                httpRequestMessage.Headers.Add("X-CM-ID", cmSuffix);
            return httpRequestMessage;
        }

        public static HttpRequestMessage CreateHttpRequest<T>(string url, T content)
        {
            // ReSharper disable once IntroduceOptionalParameters.Global
            return CreateHttpRequest(url, content, null, null);
        }
    }
}