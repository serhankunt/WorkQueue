using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
var connection = factory.CreateConnection();
var channel = connection.CreateModel();

channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);



Dictionary<string, object> headers = new Dictionary<string, object>();

headers.Add("format", "pdf");
headers.Add("shape", "a4");

var properties = channel.CreateBasicProperties();
properties.Headers = headers;

channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes("header mesajım"));

Console.WriteLine("mesaj gönderilmiştir.");

Console.ReadLine();