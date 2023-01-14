using eegs_back_end.DbModule;
using eegs_back_end.Economic_Enterprises.Market.Utility_Setup.Model;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Economic_Enterprises.Market.Utility_Setup.Repository
{
    public interface IUtilityRepository: IGlobalInterface
    {
        List<object> GetList(string type);
        List<object> GetActiveList(string type);
        UtilityModel GetByID(string id);
        bool Insert(UtilityModel model);
        bool Edit(string id, UtilityModel model);
        bool Delete(string id, string remarks);

    }
    public class UtilityRepository : IUtilityRepository
    {
        public bool Delete(string ID, string remarks)
        {
            string sql = @"update market.utility SET status = 'Cancelled', remarks = '" + remarks + @"' 
                            where utility_id = '" + ID + "' ";
            QueryModule.Execute<int>(sql);

            sql = @"update market.utility_details SET end_time = CURRENT_TIMESTAMP
                            where utility_id = '" + ID + "' and end_time is null";
            QueryModule.Execute<int>(sql);

            return true;
        }

        public bool Edit(string id, UtilityModel model)
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
                    model.utility_id = id;
                    model.status = "Active";
                    string sql = "update market.utility SET " + ObjectSqlMapping.MapUpdate<UtilityModel>() + " where utility_id = @utility_id";
                    QueryModule.Execute<int>(sql, model);

                    sql = "delete from market.utility_details where utility_id = '" + id + "'";
                    QueryModule.Execute<int>(sql, model);

                    if (model.utilities.Count > 0)
                    {

                        foreach (Utility obj in model.utilities)
                        {
                            obj.utility_id = model.utility_id;
                            sql = "insert into market.utility_details (" + ObjectSqlMapping.MapInsert<Utility>() + ")";
                            QueryModule.Execute<int>(sql, obj);
                        }
                    }

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

        public UtilityModel GetByID(string id)
        {
            string sql = @"select u.*, prop.property_name from market.utility u 

            inner join market.property prop on prop.property_id = u.property_id
            where u.utility_id = '" + id + "'";
            UtilityModel model = (UtilityModel)QueryModule.DataObject<UtilityModel>(sql);

            model.utilities = new List<Utility>();

            sql = @"select * from market.utility_details where utility_id = '"+ id +"'";
            model.utilities = (List<Utility>)QueryModule.DataSource<Utility>(sql);

            return model;
        }

        public List<object> GetList(string type)
        {
            string sql = @"select u.*, prop.property_name,
                           (select brgy.brgy_name from general_address.lgu_brgy_setup_temp brgy where brgy.brgy_id = prop.property_brgy_id) as brgy
                        from market.utility u
                          inner join market.property prop on prop.property_id = u.property_id
                          where u.utility_type = '" + type + @"' ";
            List<object> dt = (List<object>)QueryModule.DataSource<object>(sql);

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT market.property.property_name as q, 'pn' as tag
                    FROM market.`utility` uti
                    INNER JOIN market.property ON market.property.property_id = uti.property_id
                    WHERE uti.utility_type = '" + type + @"'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'ad' as tag
                    FROM market.`utility` uti
                    INNER JOIN market.property ON market.property.property_id = uti.property_id
                    INNER JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = market.property.property_brgy_id
                    WHERE uti.utility_type = '" + type + @"'

                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);

            //ExpandoObject form = Forms.getForm("utility-setup");

            List<object> list = new List<object>();
            list.Add(dt);
            //list.Add(form);
            list.Add(searches);
            return list;
        }

        public List<object> GetActiveList(string type)
        {
            string sql = @"select u.*, prop.property_name,
                           (select brgy.brgy_name from general_address.lgu_brgy_setup_temp brgy where brgy.brgy_id = prop.property_brgy_id) as brgy
                        from market.utility u
                          inner join market.property prop on prop.property_id = u.property_id
                          where u.utility_type = '" + type + @"' and u.status = 'Active' ";
            List<object> dt = (List<object>)QueryModule.DataSource<object>(sql);

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT market.property.property_name as q, 'pn' as tag
                    FROM market.`utility` uti
                    INNER JOIN market.property ON market.property.property_id = uti.property_id
                    WHERE uti.utility_type = '" + type + @"' AND uti.`status` = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'ad' as tag
                    FROM market.`utility` uti
                    INNER JOIN market.property ON market.property.property_id = uti.property_id
                    INNER JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = market.property.property_brgy_id
                    WHERE uti.utility_type = '" + type + @"' AND uti.`status` = 'Active'

                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);

            //ExpandoObject form = Forms.getForm("utility-setup");

            List<object> list = new List<object>();
            list.Add(dt);
            //list.Add(form);
            list.Add(searches);
            return list;
        }

        public bool Insert(UtilityModel model)
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
                    model.utility_id = Guid.NewGuid().ToString();
                    model.status = "Active";
                    string sql = "insert into market.utility (" + ObjectSqlMapping.MapInsert<UtilityModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);
                    if (model.utilities.Count > 0)
                    {
                        
                        foreach(Utility obj in model.utilities)
                        {
                            obj.utility_id = model.utility_id;
                           sql = "insert into market.utility_details (" + ObjectSqlMapping.MapInsert<Utility>() + ")";
                           res = (int)QueryModule.Execute<int>(sql, obj);
                        }
                    }
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
