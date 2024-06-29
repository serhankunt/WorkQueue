using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace Subscriber.Library
{
    public static class Subscriber
    {
        public static void ConsumeMessages(string queueName, string routeKey)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "double_queue_logs", type: ExchangeType.Direct);

            var queue = channel.QueueDeclare(queueName, false, false, false, arguments: null);

            channel.QueueBind(queue,
                exchange: "double_queue_logs",
                routingKey: routeKey);

            var consumer = new EventingBasicConsumer(channel);
            bool isQueueFull = true;
            while (isQueueFull)
            {
                bool autoAck = false;
                BasicGetResult result = channel.BasicGet(queue, autoAck);
                if (result == null)
                {
                    Console.WriteLine("Queueda işlenmemiş mesaj bulunmamaktadır.");
                    isQueueFull = false;
                }
                else
                {
                    IBasicProperties props = result.BasicProperties;
                    var body = result.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = result.RoutingKey;
                    Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
                    // acknowledge receipt of the message
                    channel.BasicAck(result.DeliveryTag, false);
                }
            }
            Console.WriteLine("Eski mesajları okuma tamamlandı. Queue dinleniyor...");

            Console.WriteLine(" [*] Waiting for new messages");
            consumer.Received += (model, ea) =>
            {
                Thread.Sleep(1000);
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                Console.WriteLine($" [x] Received '{routingKey}':'{message}'");

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue,
                                autoAck: false,
                                consumer: consumer);

            Console.WriteLine("Press [enter] to exit");
            Console.ReadLine();
        }
    }
}
