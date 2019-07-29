using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using RabbitMQ;
using RabbitMQ.Client.Events;

namespace RabbitMQToMSSQL
{
    public partial class RabbitMQToMSSQL : ServiceBase
    {
        private RabbitMQ_Connect rabbit;
        private SQLDB db;

        public RabbitMQToMSSQL()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                //db = new SQLDB(Properties.Settings.Default.MSSQLSRV_ServerName, Properties.Settings.Default.MSSQLSRV_DBName, Properties.Settings.Default.MSSQLSRV_UserName, Properties.Settings.Default.MSSQLSRV_Password);

                rabbit = new RabbitMQ_Connect(
                    Callback,
                    Properties.Settings.Default.hostname,
                    Properties.Settings.Default.port,
                    Properties.Settings.Default.virtualhost,
                    Properties.Settings.Default.username,
                    Properties.Settings.Default.password,
                    Properties.Settings.Default.queueName,
                    Properties.Settings.Default.exchangeName
                );

                try
                {
                    rabbit.Start(false);
                }
                catch (Exception ex)
                {
                    string conf = "HostName=" + rabbit.HostName +
                            ", Port=" + rabbit.Port.ToString() +
                            ", VirtualHost=" + rabbit.VirtualHost +
                            ", Username=" + rabbit.Username +
                            ", Password=" + rabbit.Password +
                            ", QueueName=" + rabbit.QueueName +
                            ", ExchangeName=" + rabbit.ExchangeName;
                    ErrorLog(ex.Message, new StackTrace(ex, true).ToString(), conf);
                    this.Stop();
                }
            }
            catch (Exception e)
            {
                using (StreamWriter f = new StreamWriter(Properties.Settings.Default.ErrorLogPath, true))
                {
                    f.WriteLine(DateTime.Now.ToString() + " -- " + e.Message);
                    f.Close();
                }
            }
        }

        protected override void OnStop()
        {
            rabbit.Stop();
        }
        private void Callback(RabbitMQ_Connect rabbit, object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            string inputStr = Encoding.UTF8.GetString(body);
            string result = null;
            try
            {
                result = db.Execute(Properties.Settings.Default.MSSQLSRV_FunctionName, inputStr);
            }
            catch (Exception ex)
            {
                ErrorLog(ex.Message, new StackTrace(ex, true).ToString());
            }
            if (null != result)
            {
                rabbit.Reply(result, ea.BasicProperties);
                rabbit.BasicAck(ea);
            }
            }
        void ErrorLog(string message, string strStackTrace, string conf = null)
        {
            using (StreamWriter f = new StreamWriter(Properties.Settings.Default.ErrorLogPath, true))
            {
                string line = DateTime.Now.ToString() + " -- " + "Ошибка: \"" + message + "\" in StackTrace(" + strStackTrace + ")";
                if (conf != null)
                    line += "\r\n" + conf;
                f.WriteLine(line);
                f.Close();
            }
        }
    }
}
