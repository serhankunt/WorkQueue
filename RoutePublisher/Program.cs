using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);

channel.QueueDeclare("deneme", false, false, false, arguments: null);


int i = 0;
string[] denemeList = ["deneme1", "deneme2"];
var severity = "";
while (i < 100)
{
    if (i % 2 == 0)
    {
        severity = denemeList[0];
    }
    else
    {
        severity = denemeList[1];
    }

    channel.QueueBind("deneme", exchange: "direct_logs", routingKey: severity);

    var message = (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Göt Ahmet" + i;
    i++;

    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchange: "direct_logs",
        routingKey: severity,
        basicProperties: null,
        body: body);

    Console.WriteLine($" [x] Sent '{severity} :'{message}'");
    Console.WriteLine("Press [enter] to exit");
}
Console.ReadLine();