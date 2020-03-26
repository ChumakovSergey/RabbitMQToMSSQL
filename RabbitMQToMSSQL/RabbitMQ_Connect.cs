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
        private Action<RabbitMQ_Connect, object, BasicDeliverEventArgs> callback;

        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;

        public SQLDB DB { get; }
        public string HostName { get; }
        public string VirtualHost { get; }
        public int Port { get; }
        public string Username { get; }
        public string Password { get; }
        public string ExchangeName { get; }
        public string QueueName { get; }
        
        public RabbitMQ_Connect(SQLDB db, Action<RabbitMQ_Connect, object, BasicDeliverEventArgs> callback, string hostName, int port, string virtualHost, string username, string password, string queueName, string exchangeName = "")
        {
            this.DB = db;
            this.callback = callback;
            this.HostName = hostName;
            this.Port = port;
            this.VirtualHost = virtualHost;
            this.Username = username;
            this.Password = password;
            this.QueueName = queueName;
            this.ExchangeName = exchangeName;

            this.factory = new ConnectionFactory()
            {
                UserName = this.Username,
                Password = this.Password,
                VirtualHost = this.VirtualHost,
                HostName = this.HostName,
                Port = this.Port
            };
            this.connection = this.factory.CreateConnection();
        }

        public void Start(bool autoAck)
        {
            this.channel = this.connection.CreateModel();
            EventingBasicConsumer consumer = new EventingBasicConsumer(this.channel);
            consumer.Received += (model, ea) => { this.callback(this, model, ea); };
            channel.BasicConsume(queue: this.QueueName,
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
                this.channel.BasicPublish(exchange: this.ExchangeName,
                                     routingKey: prop.ReplyTo,
                                     basicProperties: prop,
                                     body: body);
            }
            catch (System.ArgumentNullException ex)
            {
                string mess = ex.Message + " throw in Reply()! exchange=" + this.ExchangeName +
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
