using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

channel.QueueDeclare("hello-queue1", false, false, false);

channel.QueueBind("hello-queue1", exchange: "logs", routingKey: string.Empty);


Enumerable.Range(1, 50).ToList().ForEach(x =>
{
    var message = GetMessage(args) + x;

    #region RabbitMQ'da fiziksel kayıt için
    //IBasicProperties properties = channel.CreateBasicProperties();
    //properties.Persistent = true;
    #endregion

    var body = Encoding.UTF8.GetBytes(message);
    channel.BasicPublish(
        exchange: "logs",
        routingKey: string.Empty,
        basicProperties: null,
        body: body);


    Console.WriteLine($"[x] Sent {message}");
    Console.WriteLine("Press [enter] to exit");
});

//int i = 0;
//while (i < 100)
//{
//    var message = GetMessage(args) + i;
//    var body = Encoding.UTF8.GetBytes(message);
//    channel.BasicPublish(
//    exchange: "logs",
//    routingKey: string.Empty,
//    basicProperties: null,
//    body: body);
//    i++;

//    Console.WriteLine($"[x] Sent {message}");
//    Console.WriteLine("Press [enter] to exit");
//}
Console.ReadLine();


static string GetMessage(string[] args)
{
    return ((args.Length > 0) ? string.Join(" ", args) : "info: Göt Ahmet!");
}