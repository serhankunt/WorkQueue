using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);
channel.QueueDeclare("deneme", false, false, false, arguments: null);

var queueName = channel.QueueDeclare().QueueName;

string[] denemeList = ["deneme1", "deneme2"];


foreach (var severity in denemeList)
{
    channel.QueueBind(queue: queueName,
        exchange: "direct_logs",
        routingKey: severity);
}

Console.WriteLine(" [*] Waiting for messages");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    Thread.Sleep(1000);
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = ea.RoutingKey;
    Console.WriteLine($" [x] Received '{routingKey}':'{message}'");

    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
};

channel.BasicConsume(queue: queueName,
                    autoAck: false,
                    consumer: consumer);

Console.WriteLine("Press [enter] to exit");
Console.ReadLine();