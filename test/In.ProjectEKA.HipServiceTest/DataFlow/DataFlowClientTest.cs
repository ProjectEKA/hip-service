namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Bogus;
    using HipService.DataFlow.CryptoHelper;
    using In.ProjectEKA.DefaultHip.DataFlow;
    using In.ProjectEKA.HipService.DataFlow;
    using Builder;
    using Moq;
    using Moq.Protected;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.EC;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Security;
    using Xunit;

    [Collection("Queue Listener Tests")]
    public class DataFlowClientTest
    {
        private readonly Collect collect = new Collect("observation.json");
        private readonly Mock<ICryptoHelper> cryptoHelper = new Mock<ICryptoHelper>();
        [Fact]
        private async Task ReturnSuccessOnDataFlow()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                })
                .Verifiable();
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost:8003")
            };
            var dataRequest = TestBuilder.DataFlowRequest();
            var faker = new Faker();
            
            var curve = "curve25519";
            var algorithm = "ECDH";
            var ecP = CustomNamedCurves.GetByName(curve);
            var ecSpec = new ECDomainParameters(ecP.Curve, ecP.G, ecP.N, ecP.H, ecP.GetSeed());
            var generator = (ECKeyPairGenerator)GeneratorUtilities.GetKeyPairGenerator(algorithm);
            generator.Init(new ECKeyGenerationParameters(ecSpec, new SecureRandom()));
            var keyPair = generator.GenerateKeyPair();
            
            cryptoHelper.Setup(e => e.GenerateRandomKey()).Returns(It.IsAny<string>());
            cryptoHelper.Setup(e => e.GenerateKeyPair(curve,algorithm)).Returns(keyPair);
            cryptoHelper.Setup(e => e.EncryptData(faker.Random.Words(32),
                It.IsAny<AsymmetricCipherKeyPair>(), faker.Random.Words(32),
                faker.Random.Words(32), faker.Random.Words(32),
                curve, algorithm)).Returns(faker.Random.Words(32));
            cryptoHelper.Setup(e=>e.GetPublicKey(keyPair)).Returns(faker.Random.Words(32));
            var dataFlowClient = new DataFlowClient(collect, httpClient, cryptoHelper.Object);
            await dataFlowClient.HandleMessagingQueueResult(dataRequest);
            var expectedUri = new Uri("http://localhost:8003/data/notification");
            cryptoHelper.Verify();
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}