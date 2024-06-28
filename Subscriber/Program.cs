using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);
channel.QueueDeclare("hello-queue1", false, false, false, arguments: null);

//prefetchSize Mesaj boyutunu belirtir.0 değeri mesajın boyutuyla ilgilenmediğimizi belirtir.
//prefetchCount mesaj dağıtım adetini ifade eder.Kaç kaç gelsin.
//global: true=> tüm consumerların aynı anda prefetchCount parametresinde belirtilen değer kadar mesaj tüketebileceğini ifade eder.6mesaj 3 subscriber var ise 2şer şekilde dağıtımı gerçekleştiriyor.
//        false=>her bir consumer'ın işleme sürecinde diğer consumerlardan bağımsız bir şekilde kaç mesaj alacağı belirtilir.

var queueName = channel.QueueDeclare().QueueName;
channel.QueueBind(
    queue: queueName,
    exchange: "logs",
    routingKey: string.Empty);

Console.WriteLine("[*] waiting for logs");

channel.BasicQos(prefetchSize: 0, prefetchCount: 5, global: false);
var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, ea) =>
{
    Thread.Sleep(1000);
    byte[] body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[x] {message}");
    Console.WriteLine(ea.DeliveryTag);
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
    //ea.DeliveryTag consumer'ın aldığı her mesaj için eşsiz bir tanımlayıcı görevi gören bir değerdir.BasisAck metodu ile mesajı onaylarken bu değer kullanılır.
    //multiple değeri false=> sadece tek bir mesajı onaylayacağını belirtir.ea.DeliveryTag parametresine hangi değeri verirseniz verin sadece o değere sahip mesaj onaylanacaktır.
    //                true=> ea.DeliveryTag parametresine kadar gelen tüm mesajlar onaylanır.

};

channel.BasicConsume(
    queue: queueName,
    autoAck: false,
    consumer: consumer);

Console.WriteLine("Press [enter] to exit");

Console.ReadLine();