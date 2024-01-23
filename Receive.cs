using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "codap-queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

Console.WriteLine(" [*] Waiting for messages.");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] received a one message.");
    Console.WriteLine($" [v] Message: '{message}'");

    int dots = message.Split('.').Length - 1;
    Thread.Sleep(dots * 3000);

    Console.WriteLine(" [x] done.\n");
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false );
};
channel.BasicConsume(queue: "codap-queue", autoAck: false, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.\n");
Console.ReadLine();