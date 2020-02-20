namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class FakeHttpMessageHandler : DelegatingHandler
    {
        private readonly HttpResponseMessage fakeResponse;

        public FakeHttpMessageHandler(HttpResponseMessage responseMessage)
        {
            fakeResponse = responseMessage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return await Task.FromResult(fakeResponse);
        }
    }
}