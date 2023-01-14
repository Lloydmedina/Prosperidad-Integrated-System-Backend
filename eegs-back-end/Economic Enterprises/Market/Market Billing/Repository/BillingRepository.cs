using eegs_back_end.DbModule;
using eegs_back_end.Economic_Enterprises.Market.Market_Billing.Model;
using eegs_back_end.Economic_Enterprises.Market.Property_Setup.Model;
using eegs_back_end.Economic_Enterprises.Market.Property_Setup.Repository;
using eegs_back_end.Economic_Enterprises.Market.Tenant_Profiling.Model;
using eegs_back_end.Economic_Enterprises.Market.Tenant_Profiling.Repository;
using eegs_back_end.Economic_Enterprises.Market.Utility_Setup.Model;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static eegs_back_end.Economic_Enterprises.Market.Market_Billing.Repository.BillingRepository;

namespace eegs_back_end.Economic_Enterprises.Market.Market_Billing.Repository
{
    public interface IBillingRepository : IGlobalInterface
    {
        List<object> GetList(string dateFrom, string dateTo);
        List<object> GetPrintList();
        List<ExpandableList> GetTenantProfiles();
        List<object> GetUtilityRate(string property_id);
        BillingModel GetByID(string ID);
        List<object> GetPrintByID(string ID);
        bool Insert(BillingModel model);
        bool Edit(string ID, BillingModel model);
        bool Delete(string ID, string remarks);
        bool Post(string id);
    }
    public class BillingRepository : IBillingRepository
    {
        public class ExpandableList
        {
            public string key { get; set; }
            public string name { get; set; }
            public string area { get; set; }
            public string tenant { get; set; }
            public string tenant_profile_id { get; set; }
            public string contact { get; set; }
            public string address { get; set; }
            public string amt { get; set; }
            public string section { get; set; }
            public bool expand { get; set; }
            public List<ExpandableList> children { get; set; }

        }
        public bool Delete(string ID, string remarks)
        {
            throw new NotImplementedException();
        }

        public bool Edit(string ID, BillingModel model)
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
                    model.billing_id = ID;
                    model.billing_water.billing_id = ID;
                    model.billing_electricity.billing_id = ID;
                    model.billing_status = new BillingStatus();
                    model.billing_status.billing_id = model.billing_id;
                    model.billing_status.status_id = 1;
                    model.billing_status.prev_record = JsonSerializer.Serialize(GetByID(ID));
                    model.billing_status.activity = "Updated";
                    model.billing_status.user_id = GlobalObject.user_id;

                    string sql = "update market.billing_main SET " + ObjectSqlMapping.MapUpdate<BillingModel>() + " where billing_id = @billing_id";
                    QueryModule.Execute<int>(sql, model);

                    sql = "update market.billing_main_status SET end_time = CURRENT_TIMESTAMP where billing_id = @billing_id and end_time is null";
                    QueryModule.Execute<int>(sql, new { billing_id = ID });

                    sql = "insert into market.billing_main_status (" + ObjectSqlMapping.MapInsert<BillingStatus>() + ")";
                    QueryModule.Execute<int>(sql, model.billing_status);

                    sql = "update market.billing_electricity SET " + ObjectSqlMapping.MapUpdate<BillingUtility>() + " where billing_id = @billing_id";
                    QueryModule.Execute<int>(sql, model.billing_electricity);

                    sql = "update market.billing_water SET " + ObjectSqlMapping.MapUpdate<BillingUtility>() + " where billing_id = @billing_id";
                    QueryModule.Execute<int>(sql, model.billing_water);

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

        public BillingModel GetByID(string ID)
        {
            string sql = @"select * from market.billing_main where billing_id = '" + ID +"'";
            BillingModel model = (BillingModel)QueryModule.DataObject<BillingModel>(sql);

            sql = @"select * from market.billing_main_status where billing_id = '" + ID + "'";
            model.billing_status = (BillingStatus)QueryModule.DataObject<BillingStatus>(sql);

            sql = @"select * from market.billing_electricity where billing_id = '" + ID + "'";
            model.billing_electricity = (BillingUtility)QueryModule.DataObject<BillingUtility>(sql);

            sql = @"select * from market.billing_water where billing_id = '" + ID + "' ";
            model.billing_water = (BillingUtility)QueryModule.DataObject<BillingUtility>(sql);

            return model;
        }

        public List<object> GetList(string dateFrom, string dateTo)
        {
            string dateFilter = "";
            if(dateFrom != "" && dateTo != "")
            {
                dateFilter = "and Date_Format(bill.billing_date, '%m-%d-%Y') BETWEEN '" + dateFrom + "' and '" + dateTo + "'  ";
            }
            string sql = @"select bill.*, status.name as `rec_stat`, tenant.tenant_name, prop.property_name from market.billing_main bill
                        inner join market.billing_main_status stat on stat.billing_id = bill.billing_id
                        inner join market.status status on status.status_id = stat.status_id
                        inner join market.tenant_profile tenant on tenant.tenant_profile_id = bill.tenant_profile_id
                        inner join market.property prop on prop.property_id = bill.property_id
                        and stat.end_time is null
                        " + dateFilter + "";
            List<object> data = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("market-billing");


            List<object> list = new List<object>();
            list.Add(data);
            list.Add(form);
            return list;
        }

        public List<object> GetPrintList()
        {
            throw new NotImplementedException();
        }

        public bool Insert(BillingModel model)
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
                    model.billing_id = Guid.NewGuid().ToString();
                    model.form_trans_no = FormNumberGenerator.generateFormNumber("market-billing");
                    model.billing_water.billing_id = model.billing_id;
                    model.billing_electricity.billing_id = model.billing_id;
                    model.billing_status = new BillingStatus();
                    model.billing_status.billing_id = model.billing_id;
                    model.billing_status.status_id = 3;
                    model.billing_status.activity = "Added";
                    model.billing_status.user_id = GlobalObject.user_id;

                    string sql = "insert into market.billing_main (" + ObjectSqlMapping.MapInsert<BillingModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);


                    sql = "insert into market.billing_main_status (" + ObjectSqlMapping.MapInsert<BillingStatus>() + ")";
                    QueryModule.Execute<int>(sql, model.billing_status);


                    sql = "insert into market.billing_water (" + ObjectSqlMapping.MapInsert<BillingUtility>() + ")";
                    QueryModule.Execute<int>(sql, model.billing_water);


                    sql = "insert into market.billing_electricity (" + ObjectSqlMapping.MapInsert<BillingUtility>() + ")";
                    QueryModule.Execute<int>(sql, model.billing_electricity);

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

        public List<ExpandableList> GetTenantProfiles()
        {

            string sql = @"SELECT prop.property_id as `key`, prop.property_name as `name`, prop.property_area as `area`, true as `expand`
          -- prop_amt.initial_amount as `amt` 

                FROM market.`property` prop 
            inner join market.property_amount prop_amt on prop_amt.property_id = prop.property_id
            inner join market.property floor on floor.parent_id = prop.property_id
            left join market.tenant_profile_dtl floorTP on floorTP.property_id = floor.property_id and floorTP.end_time is null
            inner join market.property stall on stall.parent_id = floor.property_id
            inner join market.tenant_profile_dtl stallTP on stallTP.property_id = stall.property_id and stallTP.end_time is null

            where prop.property_type_id = 4 
            group by `key`, `name`, `area`, `expand`";
            List<ExpandableList> list = (List<ExpandableList>)QueryModule.DataSource<ExpandableList>(sql);

            foreach (ExpandableList obj in list)
            {
                obj.children = new List<ExpandableList>();
                sql = @"SELECT
	                            prop.property_id AS `key`,
	                            prop.property_name AS `name`,
	                            TRUE AS `expand`,
	                            tp.tenant_name AS `tenant`,
	                            tp.tenant_contact AS `contact`,
	                            tp.tenant_address AS `address`,
	                            prop.property_area AS `area`,
	                            tpd1.rental_amount AS `amt`,
                                tp.tenant_profile_id AS `tenant_profile_id`,
	                            (select t.property_name from market.property t
	                             where t.parent_id = prop.property_id and (t.property_id = tpd2.property_id or t.property_id = tpd1.property_id)) as `child`
                            FROM
	                            market.`property` prop
                            INNER JOIN market.property_amount prop_amt ON prop_amt.property_id = prop.property_id
                            INNER JOIN market.property ch on ch.parent_id = prop.property_id
                            LEFT JOIN market.tenant_profile_dtl tpd1 ON tpd1.property_id = prop.property_id 
                            LEFT JOIN market.tenant_profile_dtl tpd2 ON tpd2.property_id = ch.property_id
                            AND tpd2.end_time IS NULL
                            LEFT JOIN market.tenant_profile tp ON tp.tenant_profile_id = tpd1.main_id
                            LEFT JOIN market.tenant_profile_sub sub ON sub.dtl_id = tpd1.dtl_id
                            AND sub.end_time IS NULL
                            left join market.billing_main bill on bill.property_id = prop.property_id

                            where bill.property_id is NULL and
	                        prop.parent_id = '" + obj.key + @"'and tpd2.property_id is not NULL
                            ORDER BY `name` asc ";
                obj.children = (List<ExpandableList>)QueryModule.DataSource<ExpandableList>(sql);


                if (obj.children != null && obj.children.Count > 0)
                {
                    foreach (ExpandableList floor in obj.children)
                    {
                        sql = @"SELECT
	                                prop.property_id AS `key`,
	                                prop.property_name AS `name`,
	                                TRUE AS `expand`,
	                                tp.tenant_name AS `tenant`,
	                                tp.tenant_contact AS `contact`,
	                                tp.tenant_address AS `address`,
                                    tp.tenant_profile_id AS `tenant_profile_id`,
	                                prop.property_area AS `area`,
	                                tpd.rental_amount AS `amt`
                                FROM
	                                market.`property` prop
                                INNER JOIN market.property_amount prop_amt ON prop_amt.property_id = prop.property_id
                                INNER JOIN market.tenant_profile_dtl tpd ON tpd.property_id = prop.property_id
                                AND tpd.end_time IS NULL
                                INNER JOIN market.tenant_profile tp ON tp.tenant_profile_id = tpd.main_id
                                LEFT JOIN market.tenant_profile_sub sub ON sub.dtl_id = tpd.dtl_id
                                AND sub.end_time IS NULL
                                LEFT JOIN general.person person ON person.person_guid = tp.tenant_id
                                left join market.billing_main bill on bill.property_id = prop.property_id

                                where bill.property_id is NULL and
	                                prop.parent_id = '" + floor.key + @"'
								group by `area`, `amt`, `tenant`, `expand`, `name`, `key`
								order by `name` asc";
                        floor.children = (List<ExpandableList>)QueryModule.DataSource<ExpandableList>(sql);
                    }
                }
            }
            return list;
        }
        public List<object> GetUtilityRate(string property_id)
        {
            string sql = @"SELECT dtl.* FROM market.`utility_details` dtl
            inner join market.utility util on util.utility_id = dtl.utility_id
            where util.property_id = '" + property_id + @"'
            and util.utility_type = 'Water'";
            List<Utility> waterUtil = (List<Utility>)QueryModule.DataSource<Utility>(sql);

            sql = @"SELECT dtl.* FROM market.`utility_details` dtl
            inner join market.utility util on util.utility_id = dtl.utility_id
            where util.property_id = '" + property_id + @"'
            and util.utility_type = 'Electricity'";
            List<Utility> electricUtil = (List<Utility>)QueryModule.DataSource<Utility>(sql);

            List<object> list = new List<object>();

            list.Add(new { waterUtil, electricUtil });

            return list;

        }

        public bool Post(string id)
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
                    BillingStatus statusModel = new BillingStatus();
                    string sql = "update market.billing_main set status = 'Posted' where billing_id = '" + id + "'";
                    QueryModule.Execute<int>(sql);
                    statusModel.billing_id = id;
                    statusModel.status_id = 4;
                    statusModel.activity = "Post";
                    statusModel.user_id = GlobalObject.user_id;
                    sql = "update market.billing_main_status set end_time = CURRENT_TIMESTAMP where end_time is null and billing_id = '" + id + "'";
                    QueryModule.Execute<int>(sql);


                    sql = "insert into market.billing_main_status (" + ObjectSqlMapping.MapInsert<BillingStatus>() + ")";
                    QueryModule.Execute<int>(sql, statusModel);


                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }
            return true;


        }


        //public List<object> GetPrintByTenant(string ID)
        //{

        //    string sql = "select tenant_profile_id market.tenant where tenant_id =";

        //}

        public List<object> GetPrintByID(string ID)
        {
            BillingModel billing = GetByID(ID);
            PropertyRepository propRepo = new PropertyRepository();
            PropertyModel prop = propRepo.GetByID(billing.property_id);
            TenantProfileRepository tntRepo = new TenantProfileRepository();
            TenantProfileModel tenant = tntRepo.GetByID(billing.tenant_profile_id);

            PropertyModel bldg = propRepo.GetByID(prop.property_bldg_id);
            PropertyModel floor = new PropertyModel();
            if(prop.property_type_id == 1)
            {
                floor = propRepo.GetByID(prop.parent_id);
            }
            
            List<object> list = new List<object>();


            list.Add( new {billing, prop, tenant });
            list.Add(new { bldg, floor });
            return list;
        }
    }
}
