using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using In.ProjectEKA.HipLibrary.Patient;
using In.ProjectEKA.HipService.Logger;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace In.ProjectEKA.HipService.DataFlow
{
    internal class MessagingQueueListener : BackgroundService
    {
        private readonly DefaultObjectPool<IModel> objectPool;
        private IModel channel;
        private readonly ICollect collect;
        private readonly HttpClient httpClient;

        public MessagingQueueListener(IPooledObjectPolicy<IModel> objectPolicy, ICollect collect, HttpClient httpClient)
        {
            this.collect = collect;
            this.httpClient = httpClient;
            objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 2);
            SetupMessagingQueue();
        }

        private void SetupMessagingQueue()
        {
            channel = objectPool.Get();
            channel.ExchangeDeclare(MessagingQueueConstants.DataRequestExchangeName, ExchangeType.Topic, true);
            channel.QueueDeclare(
                MessagingQueueConstants.DataRequestRoutingKey,
                true,
                true,
                true,
                null);
            channel.QueueBind(
                MessagingQueueConstants.DataRequestRoutingKey,
                MessagingQueueConstants.DataRequestExchangeName,
                MessagingQueueConstants.DataRequestRoutingKey,
                null);
            channel.BasicQos(0, 1, false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var dataFlowMessage =
                    JsonConvert.DeserializeObject<HipLibrary.Patient.Model.DataRequest>(message);
                HandleMessagingQueueResult(dataFlowMessage);
                channel.BasicAck(ea.DeliveryTag, false);
            };
            channel.BasicConsume(MessagingQueueConstants.DataRequestRoutingKey, false, consumer);
            return Task.CompletedTask;
        }

        private async void HandleMessagingQueueResult(HipLibrary.Patient.Model.DataRequest dataRequest)
        {
            (await collect.CollectData(dataRequest))
                .Map(async entries =>
                {
                    var healthRecordEntries = entries.Bundles
                        .Select(bundle => new Entry(
                            JsonConvert.SerializeObject(bundle, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                ContractResolver = new DefaultContractResolver
                                {
                                    NamingStrategy = new CamelCaseNamingStrategy()
                                }
                            }),
                            "application/json",
                            "MD5"))
                        .ToList();
                    await SendDataToHiu(new DataResponse(dataRequest.TransactionId, healthRecordEntries),
                        dataRequest.CallBackUrl);
                    return Task.CompletedTask;
                });
        }

        private async Task SendDataToHiu(DataResponse dataResponse, string callBackUrl)
        {
            try
            {
                await httpClient.PostAsync($"{callBackUrl}/data/notification", CreateHttpContent(dataResponse))
                    .ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, exception.StackTrace);
            }
        }

        private static HttpContent CreateHttpContent<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}