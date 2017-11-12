using System;
using System.IO;
using System.Threading.Tasks;
using EHR.ServerEvent.Subscriber.Config;
using EHR.ServerEvent.Subscriber.Contract;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EHR.ServerEvent.Subscriber
{
    
    public class RabbitMessageProcessor<TSrc, TDst>: IMessageProcessor<TSrc>
    {
        private readonly IWriter<TDst> _writer;
        private readonly ITransformer<TSrc, TDst> _transformer;
        private readonly ILogger _logger;
        private readonly RabbitConfig _rabbitConfig;
        private RabbitMQ.Client.IModel _channel;
        private RabbitMQ.Client.IConnection _connection;
        private readonly EventingBasicConsumer _consumer;
        private readonly string _consumerTag;
        

        public RabbitMessageProcessor(
            ITransformer<TSrc, TDst> transformer,
            IWriter<TDst> writer,
            ILoggerFactory loggerFactory, 
            IOptions<RabbitConfig> options,
            IConnection connection)
        {
            _writer = writer;
            _connection = connection;
            _logger = loggerFactory.CreateLogger(GetType());
            _rabbitConfig = options.Value;
            _transformer = transformer;
            _channel = connection.CreateModel();
            _consumer = new EventingBasicConsumer(_channel);
            _consumerTag = Guid.NewGuid().ToString();
        }


        public event ProcessHandler<TSrc> OnProcessed;
        public event ErrorHandler<TSrc> OnProcessError;

        public void Start()
        {
            _consumer.Received += ProcessMessage;
            
            _channel.BasicConsume(
                queue: _rabbitConfig.Queue,
                autoAck: false,
                exclusive: false,
                consumerTag: _consumerTag,
                consumer: _consumer);

        }
        

        public void Stop()
        {
            if (_consumer != null)
            {
                _consumer.Received -= ProcessMessage;
            }
            if (_channel?.IsOpen==true)
            {
                _channel.BasicCancel(_consumerTag);
            }
        }

        private static string MessageInfo(BasicDeliverEventArgs e)
        {
            return $@"
DeliveryTag {e.DeliveryTag}
RoutingKey {e.RoutingKey}
Redelivered {e.Redelivered}";
        }

        public void ProcessMessage(object sender, BasicDeliverEventArgs e)
        {
            var mi = MessageInfo(e);
            try
            {
                _logger.LogInformation($@"Got message {mi}");

                TSrc src;
                using (var ms = new MemoryStream(e.Body))
                {
                    src = ProtoBuf.Serializer.Deserialize<TSrc>(ms);
                    _logger.LogInformation($@"Message {mi}
JSON view: {JsonConvert.SerializeObject(src, Formatting.Indented)}");
                }                
                var dst = _transformer.Transform(src);                
                _writer.WriteAsync(dst).Wait();    
                _channel.BasicAck(e.DeliveryTag, false);
                OnProcessed?.Invoke(this, new MessageProcessedEventArgs<TSrc>(src));
                _logger.LogInformation($"Message processed {mi}");

            }
            catch (Exception ex)
            {
                _logger.LogError($@"Error message process {mi}
Requeue after {_rabbitConfig.RequeueOnErrorAfter}
Error {ex.Message}. {ex}");
                OnProcessError?.Invoke(this, new MessageProcessErrorEventArgs(ex));
                Task.Run(async () =>
                {
                    await Task.Delay(_rabbitConfig.RequeueOnErrorAfter);

                    _channel?.BasicNack(e.DeliveryTag, false, true);
                    _logger.LogInformation($@"Reqeue message {mi}");
                });
                
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _channel = null;
            _connection?.Dispose();
            _connection = null;
        }
    }
}