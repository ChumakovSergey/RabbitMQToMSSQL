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
        private string connectionString;
        /// <summary>
        /// Создает объект операций с БД SQL на сервере PotrebDB
        /// </summary>
        /// <param name="ServerName">Адрес сервера</param>
        /// <param name="DBName">Имя базы даных</param>
        /// <param name="UserName">Имя пользователя</param>
        /// <param name="Password">Пароль</param>
        /// <param name="statusReport">Callback функция которой передаются параметры (Метка, ответ, SQL команда, количество затронутых строк)</param>
        public SQLDB(string ServerName, string DBName, string UserName, string Password)
        {
            connectionString = "Persist Security Info=False;" +
                                "User ID=" + UserName + ";Password=" + Password + ";" +
                                "Initial Catalog=" + DBName + ";" +
                                "Server=" + ServerName;
        }
        /// <summary>
        /// Выполняет функцию SQL-сервера
        /// </summary>
        /// <param name="functionName">Имя функции</param>
        /// <param name="functionInputVariable">Входной параметр для функции</param>
        /// <returns></returns>
        public string Execute(string functionName, string functionInputVariable)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "exec " + functionName + " N'" + functionInputVariable + "'";
                SqlCommand cmd = new SqlCommand(sql, connection);
                //Выполняем запрос
                string result = null;
                if (cmd.ExecuteScalar() != null)
                    result = cmd.ExecuteScalar().ToString();
                connection.Close();
                return result;
            }
        }

    }
}
