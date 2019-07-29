using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.IO;
using System.Diagnostics;

namespace RabbitMQToMSSQL
{
    class RabbitMQ_Connect
    {
        private string hostName;
        private string virtualHost;
        private int port;
        private string username;
        private string password;
        private string exchangeName;
        private string queueName;

        private Action<RabbitMQ_Connect, object, BasicDeliverEventArgs> callback;

        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;

        public string HostName { get => hostName; }
        public string VirtualHost { get => virtualHost; }
        public int Port { get => port; }
        public string Username { get => username; }
        public string Password { get => password; }
        public string ExchangeName { get => exchangeName; }
        public string QueueName { get => queueName; }

        public RabbitMQ_Connect(Action<RabbitMQ_Connect, object, BasicDeliverEventArgs> callback, string hostName, int port, string virtualHost, string username, string password, string queueName, string exchangeName = "")
        {
            this.callback = callback;
            this.hostName = hostName;
            this.port = port;
            this.virtualHost = virtualHost;
            this.username = username;
            this.password = password;
            this.queueName = queueName;
            this.exchangeName = exchangeName;

            this.factory = new ConnectionFactory()
            {
                UserName = this.username,
                Password = this.password,
                VirtualHost = this.virtualHost,
                HostName = this.hostName,
                Port = this.port
            };
            this.connection = this.factory.CreateConnection();
        }

        public void Start(bool autoAck)
        {
            this.channel = this.connection.CreateModel();
            EventingBasicConsumer consumer = new EventingBasicConsumer(this.channel);
            consumer.Received += (model, ea) => { this.callback(this, model, ea); };
            channel.BasicConsume(queue: this.queueName,
                                 autoAck: autoAck,
                                 consumer: consumer);
        }
        public void Stop()
        {
            if (this.channel != null && this.channel.IsOpen)
                this.channel.Close();
            if (this.connection != null && this.connection.IsOpen)
                this.connection.Close();
        }
        public void Reply(string message, IBasicProperties prop)
        {
            //Выходим, если prop.ReplyTo пустой
            if (prop.ReplyTo == null || prop.ReplyTo == "")
                return;

            byte[] body = Encoding.UTF8.GetBytes(message);
            try
            {
                this.channel.BasicPublish(exchange: this.exchangeName,
                                     routingKey: prop.ReplyTo,
                                     basicProperties: prop,
                                     body: body);
            }
            catch (System.ArgumentNullException ex)
            {
                string mess = ex.Message + " throw in Reply()! exchange=" + this.exchangeName +
                        ", routingKey=" + prop.ReplyTo +
                        ", basicProperties=" + prop.ToString() +
                        ", message=" + message;
                throw new RabbitMQTOMSSQLExeption(mess);
            }
        }
        public void BasicAck(BasicDeliverEventArgs ea)
        {
            this.channel.BasicAck(ea.DeliveryTag, false);
        }
    }

    class RabbitMQTOMSSQLExeption : ApplicationException
    {
        private string message;
        /// <summary>
        /// Текст сообщения об ошибке
        /// </summary>
        /// <param name="message"></param>
        public RabbitMQTOMSSQLExeption(string message)
        {
            this.message = message;
        }
        /// <summary>
        /// Текст сообщения об ошибке
        /// </summary>
        public override string Message { get => this.message; }
    }
}
