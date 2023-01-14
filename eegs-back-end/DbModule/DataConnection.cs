using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using Dapper;
using eegs_back_end.GlobalHandler.Error.ErrorException;

namespace eegs_back_end.DbModule
{
    public static class DataConnection
    {
        public static MySqlConnection conn { get; set; }
        public static string ConnectStr  {get;set;}
        
    }
    public static class QueryModule
    {
        public static MySqlConnection connection = DataConnection.conn;
      
        public static object DataSource<T>(string sql, object param = null)
        {
            using (var connection = new MySqlConnection(DataConnection.ConnectStr)) { 

                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
                connection.Open();
                object data = connection.Query<T>(sql, param);

                return data;
            }
        }
        public static object DataObject<T>(string sql, object param = null, bool isTransactional = false)
        {

            try
            {
                if (!isTransactional)
                {
                    using (var connection = new MySqlConnection(DataConnection.ConnectStr))
                    {

                        if (connection.State == System.Data.ConnectionState.Open)
                        {
                            connection.Close();
                        }
                        connection.Open();

                        var data = connection.QueryFirstOrDefault<T>(sql, param);

                        return data;

                    }
                }
                else
                {

                    var data = connection.QueryFirstOrDefault<T>(sql, param);
                    return data;
                }
              

            }
            catch (Exception ex)
            {
                Console.WriteLine(sql);
                throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);
            }
        }
        public static object Execute<T>(string sql, object param = null)
        {
            try
            {
                var data = connection.Execute(sql, param);
                return data;
            }
            catch (Exception ex)
            {
                throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}


