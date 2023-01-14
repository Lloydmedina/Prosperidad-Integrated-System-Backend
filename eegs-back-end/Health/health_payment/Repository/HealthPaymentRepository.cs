using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using eegs_back_end.Health.health_payment.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;

namespace eegs_back_end.Health.health_payment.Repository
{
    public interface IHealthPaymentRepository : IGlobalInterface
    {

        bool Insert(HealthCardPayment model);

    }
    public class HealthPaymentRepository : FormNumberGenerator, IHealthPaymentRepository
    {


        public bool Insert(HealthCardPayment model)
        {

            if (QueryModule.connection.State == System.Data.ConnectionState.Open)
            {
                QueryModule.connection.Close();
            }
            QueryModule.connection.Open();
            using (var medexam = QueryModule.connection.BeginTransaction())
            {
                try
                {
                    model.payment_id = Guid.NewGuid().ToString();
                    string sql = "insert into health.health_payment(" + ObjectSqlMapping.MapInsert<HealthCardPayment>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);
                    medexam.Commit();
                }
                catch (Exception ex)
                {
                    medexam.Rollback();
                    throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);
                }
            }
            QueryModule.connection.Close();
            return true;
        }
    }
}
