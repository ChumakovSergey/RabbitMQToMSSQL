using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace RabbitMQToMSSQL
{
    class SQLDB
    {
        public string ConnectionString { get; }
        private string functionName;
        private bool useNvarchar;
        /// <summary>
        /// Создает объект операций с БД SQL на сервере PotrebDB
        /// </summary>
        /// <param name="ServerName">Адрес сервера</param>
        /// <param name="DBName">Имя базы даных</param>
        /// <param name="UserName">Имя пользователя</param>
        /// <param name="Password">Пароль</param>
        /// <param name="functionName">Функция, вызываемая в процедуре Execute</param>
        /// <param name="useNvarchar">Использование nvarchar типа входного параметра для функции</param>
        public SQLDB(string ServerName, string DBName, string UserName, string Password, string functionName, bool useNvarchar = false)
        {
            this.ConnectionString = "Persist Security Info=False;" +
                                "User ID=" + UserName + ";Password=" + Password + ";" +
                                "Initial Catalog=" + DBName + ";" +
                                "Server=" + ServerName;
            this.functionName = functionName;
            this.useNvarchar = useNvarchar;
        }
        /// <summary>
        /// Выполняет функцию SQL-сервера
        /// </summary>
        /// <param name="functionName">Имя функции</param>
        /// <param name="functionInputVariable">Входной параметр для функции</param>
        /// <returns></returns>
        public string Execute(string functionInputVariable)
        {
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();
                string N = this.useNvarchar ? "N" : "";
                string sql = "exec " + this.functionName + " " + N + "'" + functionInputVariable.Replace("'", "''") + "'";
                SqlCommand cmd = new SqlCommand(sql, connection);
                //Устанавливаем timeout 300 секунд
                cmd.CommandTimeout = 300;
                //Выполняем запрос
                var result = cmd.ExecuteScalar();
                if (null != result)
                    return result.ToString();
                else
                    return null;
            }
        }

    }
}
