namespace In.ProjectEKA.HipServiceTest.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using HipService.DataFlow.CryptoHelper;
    using In.ProjectEKA.HipService.DataFlow;
    using Builder;
    using Moq;
    using Moq.Protected;
    using Optional;
    using Xunit;

    [Collection("Data Flow Client Tests")]
    public class DataFlowClientTest
    {
        private readonly Mock<ICryptoHelper> cryptoHelper = new Mock<ICryptoHelper>();

        [Fact]
        private void ShouldReturnDataComponent()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var dataRequest = TestBuilder.DataRequest(TestBuilder.Faker().Random.Hash());
            var content = TestBuilder.Faker().Random.String();
            var checksum = TestBuilder.Faker().Random.Hash();
            var entries = Option.Some(new List<Entry>
                {
                    new Entry(content, "application/json", checksum, null)
                }
                .AsEnumerable());
            var expectedUri = new Uri("http://callback/data/notification");
            var dataFlowClient = new DataFlowClient(httpClient, cryptoHelper.Object);

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
            // var faker = new Faker();
            // var httpClient = new HttpClient(handlerMock.Object);
            // var collect = new Mock<ICollect>();
            // var dataRequest = TestBuilder.DataRequest(TestBuilder.Faker().Random.Hash());
            // collect.Setup(iCollect => iCollect.CollectData(dataRequest))
            //     .ReturnsAsync(Option.Some(new Entries(new List<Bundle>())));
            // var curve = "curve25519";
            // var algorithm = "ECDH";
            // var ecP = CustomNamedCurves.GetByName(curve);
            // var ecSpec = new ECDomainParameters(ecP.Curve, ecP.G, ecP.N, ecP.H, ecP.GetSeed());
            // var generator = (ECKeyPairGenerator)GeneratorUtilities.GetKeyPairGenerator(algorithm);
            // generator.Init(new ECKeyGenerationParameters(ecSpec, new SecureRandom()));
            // var keyPair = generator.GenerateKeyPair();
            //
            // cryptoHelper.Setup(e => e.GenerateRandomKey()).Returns(It.IsAny<string>());
            // cryptoHelper.Setup(e => e.GenerateKeyPair(curve,algorithm)).Returns(keyPair);
            // cryptoHelper.Setup(e => e.EncryptData(faker.Random.Words(32),
            //     It.IsAny<AsymmetricCipherKeyPair>(), faker.Random.Words(32),
            //     faker.Random.Words(32), faker.Random.Words(32),
            //     curve, algorithm)).Returns(faker.Random.Words(32));
            // cryptoHelper.Setup(e=>e.GetPublicKey(keyPair)).Returns(faker.Random.Words(32));
            // var dataFlowClient = new DataFlowClient(collect.Object,
            //     httpClient,cryptoHelper.Object);
            //
            // await dataFlowClient.HandleMessagingQueueResult(dataRequest);
            // var expectedUri = new Uri("http://callback/data/notification");
            
            dataFlowClient.SendDataToHiu(dataRequest, entries, null);

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(message => message.Method == HttpMethod.Post
                                                         && message.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}