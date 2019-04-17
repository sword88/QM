using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace QM.Demo.RabbitMQ
{
    class RPCServer
    {
        public static void Main()
        {            

            var factory = new ConnectionFactory() { HostName = "127.0.0.1" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "RPC_QUEUE", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    channel.BasicQos(0, 1, false);

                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume(queue: "RPC_QUEUE", autoAck: false, consumer: consumer);


                    Console.WriteLine("START SEVICES.....");

                    consumer.Received += (model, ea) =>
                     {
                         string response = null;

                         var body = ea.Body;
                         var props = ea.BasicProperties;
                         var replyProps = channel.CreateBasicProperties();
                         replyProps.CorrelationId = props.CorrelationId;

                         try
                         {
                             var message = Encoding.UTF8.GetString(body);                             
                             response = dosomething(message);
                             Console.WriteLine(response);
                         }
                         catch (Exception e)
                         {
                             response = "";
                             Console.WriteLine(e.Message);
                         }
                         finally
                         {
                             var responseBytes = Encoding.UTF8.GetBytes(response);
                             channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                               basicProperties: replyProps, body: responseBytes);
                             channel.BasicAck(deliveryTag: ea.DeliveryTag,
                               multiple: false);
                         }

                     };

                    Console.ReadLine();
                }
            }
        }

        private static string dosomething(string msg)
        {
            QMDBHelper db = new QMDBHelper("DATA SOURCE=10.68.10.143:1537/db2dev;PASSWORD=mesft;USER ID=mesft");

            string sql = "select Asewh_Func_IRCustlotNumRoll1('IR','1929','AIRF1404ZPBF00') from dual";

            var obj = db.ExecuteScalar(CommandType.Text, sql, null);

            return msg + obj.ToString();
        }
    }
}
