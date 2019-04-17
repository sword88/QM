using Oracle.ManagedDataAccess.Client;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace QM.Demo.RabbitMQ_Client
{
    class RPCClient
    {
        public static void Main()
        {
            QMDBHelper db = new QMDBHelper("DATA SOURCE=10.68.10.143:1537/db2dev;PASSWORD=mesft;USER ID=mesft");

            var factory = new ConnectionFactory() { HostName = "localhost" };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var replyQueueName = channel.QueueDeclare().QueueName;
            var consumer = new EventingBasicConsumer(channel);

            var props = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            BlockingCollection<string> respQueue = new BlockingCollection<string>();

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    respQueue.Add(response);
                }
            };

            double i = 0;

            while (true)
            {
                Thread.Sleep(1);

                string message = "";

                var messageBytes = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(
                    exchange: "",
                    routingKey: "RPC_QUEUE",
                    basicProperties: props,
                    body: messageBytes);

                channel.BasicConsume(
                        consumer: consumer,
                        queue: replyQueueName,
                        autoAck: true);

                string result = respQueue.Take();

                Console.WriteLine(result);

                string sql = @"insert into BI_LOTLIST 
                                    (aolot,
                                     CUSTOMERLOT,
                                     STEP) 
                            values
                                    (:ao,
                                     :custlot,
                                     'NA')";

                OracleParameter[] param = new OracleParameter[]
                {
                    new OracleParameter(":ao",result),
                    new OracleParameter(":custlot","A"),
                };

                db.ExecuteNonQuery(sql, CommandType.Text, param);
            }
        }
    }
}
