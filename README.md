# RabbitMQToMSSQL
MSSQL RabbitMQ listener. Listen RabitMQ queue and execution MSSQL prosedure. It listens in the RabbitMQ queue and executes the SQL server procedure specified in the RabbitMQToMSSQL.exe.config file. The result of the procedure is sent to the new RabbitMQ queue using routing_key from the incoming message.
