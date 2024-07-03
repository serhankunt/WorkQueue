using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);
channel.BasicQos(0, 1, false);
var consumer = new EventingBasicConsumer(channel);

var queueName = channel.QueueDeclare().QueueName;

Dictionary<string, object> headers = new Dictionary<string, object>();

headers.Add("format", "pdf");
headers.Add("shape", "a4");
headers.Add("x-match", "all");

channel.QueueBind(queueName, "header-exchange", String.Empty, headers);

channel.BasicConsume(queueName, false, consumer);

Console.WriteLine("Loglar dinleniyor");

consumer.Received += (model, ea) =>
{
    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
    Console.WriteLine("Gelen Mesaj :" + message);
    channel.BasicAck(ea.DeliveryTag, false);
};

Console.ReadLine();