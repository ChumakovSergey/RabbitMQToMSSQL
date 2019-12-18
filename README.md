# RabbitMQToMSSQL
MSSQL RabbitMQ listener. Listen RabitMQ queue and execution MSSQL prosedure. It listens in the RabbitMQ queue and executes the SQL server procedure specified in the RabbitMQToMSSQL.exe.config file. The result of the procedure is sent to the new RabbitMQ queue using routing_key from the incoming message.

# How to Use
1. Edit config file RabbitMQToMSSQL.exe.config
2. Run the command prompt with administrator rights!
3. Install RabbitMQToMSSQL service with InstallUtil.exe with command: *PATH TO InstallUtil.exe*\InstallUtil.exe *PATH TO RabbitMQToMSSQL.exe*\RabbitMQToMSSQL.exe
   For uninstall RabbitMQToMSSQL service use that command: sc delete RabbitMQToMSSQL
4. Run RabbitMQToMSSQL service with command: net start RabbitMQToMSSQL