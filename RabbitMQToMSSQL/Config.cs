using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQToMSSQL
{
    class Config
    {
        public Route[] Routes { get; }
        public Config(string ErrorLogPath, Route[] Routes)
        {
            this.Routes = Routes;
        }
    }
    class Route
    {
        public string RabbitMQ_Hostname { get; }
        public string RabbitMQ_Virtualhost { get; }
        public int RabbitMQ_Port { get; }
        public string RabbitMQ_Username { get; }
        public string RabbitMQ_Password { get; }
        public string RabbitMQ_ExchangeName { get; }
        public string RabbitMQ_QueueName { get; }
        public string MSSQLSRV_ServerName { get; }
        public string MSSQLSRV_UserName { get; }
        public string MSSQLSRV_Password { get; }
        public string MSSQLSRV_DBName { get; }
        public string MSSQLSRV_FunctionName { get; }
        public bool MSSQLSRV_UseNvarchar { get; }

        public Route(string rabbitMQ_Hostname, string rabbitMQ_Virtualhost, int rabbitMQ_Port, string rabbitMQ_Username, string rabbitMQ_Password, string rabbitMQ_ExchangeName, string rabbitMQ_QueueName, string mSSQLSRV_ServerName, string mSSQLSRV_UserName, string mSSQLSRV_Password, string mSSQLSRV_DBName, string mSSQLSRV_FunctionName, bool mSSQLSRV_UseNvarchar = false)
        {
            RabbitMQ_Hostname = rabbitMQ_Hostname;
            RabbitMQ_Virtualhost = rabbitMQ_Virtualhost;
            RabbitMQ_Port = rabbitMQ_Port;
            RabbitMQ_Username = rabbitMQ_Username;
            RabbitMQ_Password = rabbitMQ_Password;
            RabbitMQ_ExchangeName = rabbitMQ_ExchangeName;
            RabbitMQ_QueueName = rabbitMQ_QueueName;
            MSSQLSRV_ServerName = mSSQLSRV_ServerName;
            MSSQLSRV_UserName = mSSQLSRV_UserName;
            MSSQLSRV_Password = mSSQLSRV_Password;
            MSSQLSRV_DBName = mSSQLSRV_DBName;
            MSSQLSRV_FunctionName = mSSQLSRV_FunctionName;
            MSSQLSRV_UseNvarchar = mSSQLSRV_UseNvarchar;
        }
    }
}
