using eegs_back_end.DbModule;
using eegs_back_end.Economic_Enterprises.Market.Delinquent_List.Model;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Economic_Enterprises.Market.Delinquent_List.Repository
{
    public interface IDelinquentRepository : IGlobalInterface
    {
        List<object> GetList();
        bool Save(DelinquentModel model);
        List<object> Generate(DateTime date);
        DelinquentModel GetRecord(string id);
    }
    public class DelinquentRepository : IDelinquentRepository
    {
        public List<object> Generate(DateTime date)
        {
            string sql = @"SELECT billing.*, tenant.tenant_name, prop.property_name FROM market.billing_main billing
                            inner join market.tenant_profile tenant on tenant.tenant_profile_id = billing.tenant_profile_id
                            inner join market.property prop on prop.property_id = billing.property_id
                           where billing.`status` = 'Unpaid' and DATE(billing.due_date) <= DATE('" + date.ToString("yyyy-MM-dd") + "')  ";
            List<object> list = (List<object>)QueryModule.DataSource<object>(sql);

            return list;
        }

        public List<object> GetList()
        {
            string sql = @"SELECT del.*, CONCAT(person.first_name,' ', person.last_name) as `user` FROM market.`delinquent_list` del
                        inner join general.users `user` on `user`.user_guid = del.user_id
                        inner join general.person person on person.person_guid = `user`.person_guid";
            List<object> list = (List<object>)QueryModule.DataSource<object>(sql);

            return list;
        }

        public DelinquentModel GetRecord(string id)
        {
            string sql = "select * from market.delinquent_list where delinquent_list_id = '" + id + "'";
            DelinquentModel model = (DelinquentModel)QueryModule.DataObject<DelinquentModel>(sql);

            return model;
        }

        public bool Save(DelinquentModel model)
        {
            model.user_id = GlobalObject.user_id;
            model.delinquent_list_id = Guid.NewGuid().ToString();
           string sql = "insert into market.delinquent_list (" + ObjectSqlMapping.MapInsert<DelinquentModel>() + ")";
            int res = (int)QueryModule.Execute<int>(sql, model);
            return true;
        }
    }
}
