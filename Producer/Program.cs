using RabbitMQ.Client;
using System.Text;
Console.Title = "Publisher";
Thread.Sleep(5000);
var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "double_queue_logs", type: ExchangeType.Direct);

var firstQueue = channel.QueueDeclare("firstQueue", false, false, false, arguments: null);
var SecondQueue = channel.QueueDeclare("secondQueue", false, false, false, arguments: null);

int i = 0;
var severity = "deneme";
var queue = "";
var routeKey = "";
while (i < 100)
{
    if (i % 2 == 0)
    {
        queue = firstQueue;
        routeKey = queue;
    }
    else
    {
        queue = SecondQueue;
        routeKey = queue;
    }

    channel.QueueBind(queue, exchange: "double_queue_logs", routingKey: routeKey);
    var message = (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Serhanı göttenzikem " + i;
    i++;

    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchange: "double_queue_logs",
        routingKey: routeKey,
        basicProperties: null,
        body: body);

    Console.WriteLine($" [x] Sent to '{queue}' - '{severity} :'{message}'");
    Console.WriteLine("Press [enter] to exit");
}
Console.ReadLine();