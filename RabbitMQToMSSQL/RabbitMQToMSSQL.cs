﻿using System;
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
using Newtonsoft.Json;

namespace RabbitMQToMSSQL
{
    public partial class RabbitMQToMSSQL : ServiceBase
    {
        private RabbitMQ_Connect[] RabbitMQ_Connections;
        private Settings settings;
        public RabbitMQToMSSQL()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                using (StreamReader r = new StreamReader(Properties.Settings.Default.RoutesPath))
                {
                    string json = r.ReadToEnd();
                    this.settings = JsonConvert.DeserializeObject<Settings>(json);
                    this.RabbitMQ_Connections = new RabbitMQ_Connect[settings.Routes.Length];
                }
                for (int i = 0; i < RabbitMQ_Connections.Length; i++)
                {
                    try
                    {
                        RabbitMQ_Connections[i] = new RabbitMQ_Connect(
                            new SQLDB(
                                settings.Routes[i].MSSQLSRV_ServerName,
                                settings.Routes[i].MSSQLSRV_DBName,
                                settings.Routes[i].MSSQLSRV_UserName,
                                settings.Routes[i].MSSQLSRV_Password,
                                settings.Routes[i].MSSQLSRV_FunctionName
                            ),
                            Callback,
                            settings.Routes[i].RabbitMQ_Hostname,
                            settings.Routes[i].RabbitMQ_Port,
                            settings.Routes[i].RabbitMQ_Virtualhost,
                            settings.Routes[i].RabbitMQ_Username,
                            settings.Routes[i].RabbitMQ_Password,
                            settings.Routes[i].RabbitMQ_QueueName,
                            settings.Routes[i].RabbitMQ_ExchangeName
                        );
                        try
                        {
                            RabbitMQ_Connections[i].Start(false);
                        }
                        catch (Exception ex)
                        {
                            string conf = "HostName=" + RabbitMQ_Connections[i].HostName +
                                    ", Port=" + RabbitMQ_Connections[i].Port.ToString() +
                                    ", VirtualHost=" + RabbitMQ_Connections[i].VirtualHost +
                                    ", Username=" + RabbitMQ_Connections[i].Username +
                                    ", Password=" + RabbitMQ_Connections[i].Password +
                                    ", QueueName=" + RabbitMQ_Connections[i].QueueName +
                                    ", ExchangeName=" + RabbitMQ_Connections[i].ExchangeName;
                            ErrorLog("In RabbitMQ_Connections[" + i.ToString() + "].Start(false) " + ex.Message, new StackTrace(ex, true).ToString(), conf);
                            this.Stop();
                        }
                    }
                    catch (Exception e)
                    {
                        string conf = "HostName=" + settings.Routes[i].RabbitMQ_Hostname +
                                    ", Port=" + settings.Routes[i].RabbitMQ_Port.ToString() +
                                    ", VirtualHost=" + settings.Routes[i].RabbitMQ_Virtualhost +
                                    ", Username=" + settings.Routes[i].RabbitMQ_Username +
                                    ", Password=" + settings.Routes[i].RabbitMQ_Password +
                                    ", QueueName=" + settings.Routes[i].RabbitMQ_QueueName +
                                    ", ExchangeName=" + settings.Routes[i].RabbitMQ_ExchangeName;
                        ErrorLog("In new RabbitMQ_Connect() " + e.Message, new StackTrace(e, true).ToString(), conf);
                        this.Stop();
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog("In new load settings file. With message: " + e.Message, new StackTrace(e, true).ToString());
                this.Stop();
            }
        }

        protected override void OnStop()
        {
            foreach(RabbitMQ_Connect rabbit in RabbitMQ_Connections)
                rabbit.Stop();
        }
        private void Callback(RabbitMQ_Connect rabbit, object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            string inputStr = Encoding.UTF8.GetString(body);
            string result = null;
            try
            {
                result = rabbit.DB.Execute(inputStr);
                if (null != result)
                    rabbit.Reply(result, ea.BasicProperties);
                rabbit.BasicAck(ea);
            }
            catch (Exception ex)
            {
                ErrorLog(ex.Message, new StackTrace(ex, true).ToString(),rabbit.DB.ConnectionString);
            }
        }
        void ErrorLog(string message, string strStackTrace, string conf = null)
        {
            using (StreamWriter f = new StreamWriter(Properties.Settings.Default.ErrorLogPath, true))
            {
                string line = DateTime.Now.ToString() + " -- " + "Error: \"" + message + "\" in StackTrace(" + strStackTrace + ")";
                if (conf != null)
                    line += "\r\nConfiguration: " + conf;
                f.WriteLine(line);
                f.Close();
            }
        }
    }
}
