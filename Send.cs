using System;
using RabbitMQ.Client;
using System.Text;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Send
{
    public class Send
    {
        public static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello1",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null
                                     );
                channel.ConfirmSelect();
                List<RequestQ> lstrequ = new List<RequestQ>();
                for(int i=1; i<10; i++)
                {
                    RequestQ reqstQ = new RequestQ()
                    {
                        REQUEST_NUM = i,
                        Received_by_RefreshWS = false,
                        Received_by_Mobility_BackEnd = false
                    };
                    lstrequ.Add(reqstQ);
                }
                var message = JsonSerializer.Serialize<List<RequestQ>>(lstrequ); 
                var body = Encoding.UTF8.GetBytes(message);
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                channel.BasicPublish(exchange: "",
                                     routingKey: "hello1",
                                     basicProperties: properties,
                                     body: body);
               
                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }

    public class RequestQ
    {
        public int REQUEST_NUM;

        public bool Received_by_RefreshWS;

        public bool Received_by_Mobility_BackEnd;
    }
}
