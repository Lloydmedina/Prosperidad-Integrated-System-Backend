using eegs_back_end.GlobalHandler.Error.ErrorException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.DbModule
{
    public class InsertUpdate
    {
        public static bool ExecuteSql(string sql, object obj)
        {
            if (QueryModule.connection.State == System.Data.ConnectionState.Open)
            {
                QueryModule.connection.Close();
            }
            QueryModule.connection.Open();
            using (var transaction = QueryModule.connection.BeginTransaction())
            {
                try
                {
                    QueryModule.Execute<int>(sql, obj);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);
                }
            }

            QueryModule.connection.Close();
            return true;
        }
    }
}
