using eegs_back_end.DbModule;
using eegs_back_end.Economic_Enterprises.Market.Tenant_Profiling.Model;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace eegs_back_end.Economic_Enterprises.Market.Tenant_Profiling.Repository
{
    public interface ITenantProfileRepository: IGlobalInterface
    {
        List<object> GetList(int? status_id = 0);
        List<object> GetPrintList();
        TenantProfileModel GetByID(string ID);
        bool Insert(TenantProfileModel model);
        bool Edit(string ID, TenantProfileModel model);
        bool Delete(string ID, string remarks);
        List<object> GetDropDown();
    }
    public class TenantProfileRepository : FormNumberGenerator, ITenantProfileRepository
    {
        public bool Delete(string ID, string remarks)
        {
            //string sql = @"update market.tenant_profile_dtl SET end_time = CURRENT_TIMESTAMP, status = 'Cancelled', remarks = '" + remarks + @"' 
            //                where dtl_id = '" + ID + "' and end_time is null";
            //QueryModule.Execute<int>(sql);

            return true;


        }

        public bool Edit(string ID, TenantProfileModel model)
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
                    model.tenant_profile_id = ID;
                    string sql = "update market.tenant_profile SET " + ObjectSqlMapping.MapUpdate<TenantProfileModel>() + " where tenant_profile_id = @tenant_profile_id";
                    QueryModule.Execute<int>(sql, model);

                    sql = "update market.tenant_profile_status SET end_time = CURRENT_TIMESTAMP where tenant_profile_id = @tenant_profile_id and end_time is null";
                    QueryModule.Execute<int>(sql, new { tenant_profile_id = ID });


                    model.status_model = new TenantProfileStatus();
                    model.status_model.tenant_profile_id = ID;
                    model.status_model.status_id = 1;
                    model.status_model.prev_record = JsonSerializer.Serialize(GetByID(ID));
                    model.status_model.activity = "Updated";
                    model.status_model.user_id = GlobalObject.user_id;

                    sql = "insert into market.tenant_profile_status (" + ObjectSqlMapping.MapInsert<TenantProfileStatus>() + ")";
                    QueryModule.Execute<int>(sql, model.status_model);

                    if(model.tenant_profile_dtl.Count > 0)
                    {
                        sql = "update market.tenant_profile_dtl SET end_time = CURRENT_TIMESTAMP, status = 'Inactive' where main_id = '" + ID + "' and end_time is null";
                        QueryModule.Execute<int>(sql);

                        foreach (TenantProfileDetail obj in model.tenant_profile_dtl)
                        {
                            sql = "update market.tenant_profile_sub SET end_time = CURRENT_TIMESTAMP where dtl_id = '" + obj.dtl_id + "' and end_time is null";
                            QueryModule.Execute<int>(sql);

                            obj.main_id = model.tenant_profile_id;
                            obj.dtl_id = Guid.NewGuid().ToString();
                            obj.status = "Active";
                            sql = "insert into market.tenant_profile_dtl (" + ObjectSqlMapping.MapInsert<TenantProfileDetail>() + ")";
                            QueryModule.Execute<int>(sql, obj);

                            if (obj.sub_tenant.Count > 0)
                            {
                                foreach (TenantProfileSub sub in obj.sub_tenant)
                                {
                                    sub.dtl_id = obj.dtl_id;
                                    sql = "insert into market.tenant_profile_sub (" + ObjectSqlMapping.MapInsert<TenantProfileSub>() + ")";
                                    QueryModule.Execute<int>(sql, sub);
                                }
                            }
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

        public TenantProfileModel GetByID(string ID)
        {
            string sql = "select * from market.tenant_profile tp where tp.tenant_profile_id = '" + ID + "'";
            TenantProfileModel model = (TenantProfileModel)QueryModule.DataObject<TenantProfileModel>(sql);

            sql = "select * from market.tenant_profile_dtl dtl where dtl.main_id = '" + ID + "' and dtl.end_time is null ";
            model.tenant_profile_dtl = (List<TenantProfileDetail>)QueryModule.DataSource<TenantProfileDetail>(sql);



            if(model.tenant_profile_dtl != null && model.tenant_profile_dtl.Count > 0)
            {
                foreach(TenantProfileDetail obj in model.tenant_profile_dtl)
                {
                    sql = "select * from market.tenant_profile_sub sub where sub.dtl_id = '" + obj.dtl_id + "'  ";
                    obj.sub_tenant = (List<TenantProfileSub>)QueryModule.DataSource<TenantProfileSub>(sql);

                    sql = @"select prop.*, 
                                (SELECT
	                                GROUP_CONCAT(sec.section_name)
                                FROM
	                                market.property_section propsec
                                INNER JOIN market.section sec ON sec.section_id = propsec.section_id
                                WHERE
	                                propsec.property_id = prop.property_id) as `section`

                            from market.property prop where
                                   
                            prop.property_id = '" + obj.property_id + "'";
                    obj.propertyInfo = (PropertyInfo)QueryModule.DataObject<PropertyInfo>(sql);
                }
            }
            return model;
        }

        public List<object> GetDropDown()
        {
            throw new NotImplementedException();
        }

        public List<object> GetList(int? status_id = 0)
        {
            string filter = "";
            if (status_id != 0)
            {
                filter = "where tpstat.status_id = " + status_id;
            }
            string sql = @"
                          SELECT
                            tp.tenant_profile_id,
                            bldg.property_name as `bldg`,
                            floor.property_name as `floor`,
                            prop.property_name,
                            dtl.rental_amount,
                            tp.tenant_name AS `tenant`,
                            stat.name AS status_rec
                            FROM
                            market.tenant_profile tp
                            INNER JOIN market.tenant_profile_dtl dtl ON dtl.main_id = tp.tenant_profile_id and dtl.end_time is NULL

                            LEFT JOIN market.property prop ON prop.property_id = dtl.property_id
                            LEFT JOIN market.property bldg on bldg.property_id = prop.property_bldg_id
                            LEFT JOIN market.property floor on floor.property_id = prop.parent_id and floor.property_type_id = 5

                            INNER JOIN market.tenant_profile_status tpstat ON tpstat.tenant_profile_id = tp.tenant_profile_id AND tpstat.end_time IS NULL
                            LEFT JOIN market.tenant_profile_sub sub ON sub.dtl_id = dtl.dtl_id and sub.end_time is NULL
                            LEFT JOIN general.person person ON person.person_guid = tp.tenant_id
                            INNER JOIN market.status stat ON stat.status_id = tpstat.status_id

                            " + filter + @"
                            GROUP BY tp.tenant_profile_id, bldg, floor, prop.property_name, dtl.rental_amount, `tenant`, dtl.dtl_id, status_rec
                            ORDER by bldg.property_name, floor.property_name, prop.property_name
                        ";
            List<object> data = (List<object>)QueryModule.DataSource<object>(sql);


            ExpandoObject form = Forms.getForm("tenant-profiling");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT prop.property_name as q, 'pn' as tag
                    FROM market.tenant_profile tp
                    INNER JOIN market.tenant_profile_dtl dtl ON dtl.main_id = tp.tenant_profile_id and dtl.end_time is NULL
                    LEFT JOIN market.property prop ON prop.property_id = dtl.property_id

                    UNION

                    SELECT tp.tenant_name as q, 'tn' as tag
                    FROM market.tenant_profile tp
                    INNER JOIN market.tenant_profile_status tpstat ON tpstat.tenant_profile_id = tp.tenant_profile_id AND tpstat.end_time IS NULL
                    where tpstat.status_id = 1

                    ) z
                    ";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);


            List<object> list = new List<object>();
            list.Add(data);
            list.Add(form);
            list.Add(searches);
            return list;
        }

        public class ExpandableList
        {
            public string key { get; set; }
            public string name { get; set; }
            public string area { get; set; }
            public string tenant { get; set; }
            public string contact { get; set; }
            public string address { get; set; }
            public string amt { get; set; }
            public string section { get; set; }
            public bool expand { get; set; }
            public List<ExpandableList> children { get; set; }

        }

        public List<object> GetPrintList()
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
                            WHERE
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
                                WHERE
	                                prop.parent_id = '" + floor.key + @"'
								group by `area`, `amt`, `tenant`, `expand`, `name`, `key`
								order by `name` asc";
                        floor.children = (List<ExpandableList>)QueryModule.DataSource<ExpandableList>(sql);
                    }
                }
            }

            ExpandoObject form = Forms.getForm("tenant-profiling");
            List<object> listObj = new List<object>();
            listObj.Add(list);
            listObj.Add(form);

            return listObj;
        }

        public bool Insert(TenantProfileModel model)
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
                    model.tenant_profile_id = Guid.NewGuid().ToString();
                    model.status = "Active";
                    model.status_model = new TenantProfileStatus();
                    model.status_model.tenant_profile_id = model.tenant_profile_id;
                    model.status_model.status_id = 1;
                    model.status_model.activity = "Added";
                    model.status_model.user_id = GlobalObject.user_id;

                    string sql = "insert into market.tenant_profile (" + ObjectSqlMapping.MapInsert<TenantProfileModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    sql = "insert into market.tenant_profile_status (" + ObjectSqlMapping.MapInsert<TenantProfileStatus>() + ")";
                    QueryModule.Execute<int>(sql, model.status_model);

                    if(model.tenant_profile_dtl.Count > 0)
                    {
                        foreach(TenantProfileDetail obj in model.tenant_profile_dtl)
                        {
                            obj.main_id = model.tenant_profile_id;
                            obj.dtl_id = Guid.NewGuid().ToString();
                            obj.transaction_date = DateTime.UtcNow;
                            obj.status = "Active";
                            sql = "insert into market.tenant_profile_dtl (" + ObjectSqlMapping.MapInsert<TenantProfileDetail>() + ")";
                            res = (int)QueryModule.Execute<int>(sql, obj);


                            if (obj.sub_tenant.Count > 0)
                            {
                                foreach(TenantProfileSub sub in obj.sub_tenant)
                                {
                                    sub.dtl_id = obj.dtl_id;
                                    sql = "insert into market.tenant_profile_sub (" + ObjectSqlMapping.MapInsert<TenantProfileSub>() + ")";
                                    res = (int)QueryModule.Execute<int>(sql, sub);
                                }
                            }
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
