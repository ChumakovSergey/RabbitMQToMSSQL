# RabbitMQToMSSQL
MSSQL RabbitMQ listener. Listen RabitMQ queue and execution MSSQL prosedure. It listens in the RabbitMQ queue and executes the SQL server procedure specified in the RabbitMQToMSSQL.exe.config file. The result of the procedure is sent to the new RabbitMQ queue using routing_key from the incoming message.

# How to config
1. Make your config.json file. See config-sample.json.
    ```json
    {
        "Routes": 
        [
            {
            "RabbitMQ_Hostname": "rabbitmq-server-host",
            "RabbitMQ_Virtualhost": "/",
            "RabbitMQ_Port": "5672",
            "RabbitMQ_Username": "guest",
            "RabbitMQ_Password": "guest",
            "RabbitMQ_ExchangeName": "",
            "RabbitMQ_QueueName": "queue_name",
            "MSSQLSRV_ServerName": "sql-server-host",
            "MSSQLSRV_UserName": "username",
            "MSSQLSRV_Password": "password",
            "MSSQLSRV_DBName": "db-name",
            "MSSQLSRV_FunctionName": "function-name",
            "MSSQLSRV_UseNvarchar": true
            }
        ],
        [
            ...
        ]
    }
    ```
    * **Routers**: Array of routers, contains one or more routes.
    * **RabbitMQ_Hostname**: The host address to your RabbitMQ server.
    * **RabbitMQ_Virtualhost**: The name of the virtualhost to your RabbitMQ server.
    * **RabbitMQ_Port**: The port to your RabbitMQ server.
    * **RabbitMQ_Username**: The username in your RabbitMQ server.
    * **RabbitMQ_Password**: The password of the user in your RabbitMQ server.
    * **RabbitMQ_ExchangeName**: The name of the exchange in your RabbitMQ server.
    * **RabbitMQ_QueueName**: Queue name in your RabbitMQ server.
    * **MSSQLSRV_ServerName**: The host address to your RabbitMQ server.
    * **MSSQLSRV_UserName**: The username in your RabbitMQ server.
    * **MSSQLSRV_Password**: The password of the user in your RabbitMQ server.
    * **MSSQLSRV_DBName**: The name of the database on your RabbitMQ server.
    * **MSSQLSRV_FunctionName**: The called function in your RabbitMQ server.
    * **MSSQLSRV_UseNvarchar**: *(Optional | Default = false)* Using nchar/nvarchar type when called `MSSQLSRV_FunctionName` function. 
        * If `true` than service call: 
            ```sql
            exec <MSSQLSRV_FunctionName> N''
            ```
        * Else service call:
            ```sql
            exec <MSSQLSRV_FunctionName> ''
            ```


2. Edit RabbitMQToMSSQL\RabbitMQToMSSQL\bin\Release\RabbitMQToMSSQL.exe.config.
    * *(Optional)* Change default error log path
        ```xml
        <setting name="ErrorLogPath" serializeAs="String">
            <value>C:\RabbitMQToMSSQLError.log</value>
        </setting>
        ```
    * **Enter the path to your config.json file here**
        ```xml
        <setting name="ConfigFilePath" serializeAs="String">
                <value>PathToYourConfigFile</value>
        </setting>
        ```


# How to Use
1. Edit config file RabbitMQToMSSQL.exe.config
2. Run the command prompt **with administrator rights!**
3. Install RabbitMQToMSSQL service with InstallUtil.exe with command: `<PathToInstallUtil.exe> <PathToRabbitMQToMSSQL.exe>`. Example:
    ```
    C:\WINDOWS\system32>C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe C:\repos\RabbitMQToMSSQL\RabbitMQToMSSQL\bin\Release\RabbitMQToMSSQL.exe
    ```
   For uninstall RabbitMQToMSSQL service use that command: 
   ```
   sc delete RabbitMQToMSSQL
   ```
4. Run RabbitMQToMSSQL service in service manager or run that command: 
    ```
    net start RabbitMQToMSSQL
    ```