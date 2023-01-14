using eegs_back_end.DbModule;
using eegs_back_end.Economic_Enterprises.Market.Rental_Application.Model;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Economic_Enterprises.Market.Rental_Application.Repository
{
    public interface IRentalApplicationRepository : IGlobalInterface
    {
        List<object> GetList(int? status_id = 0, string? type = "");
        RentalApplicationModel GetByID(string ID);
        bool Insert(RentalApplicationModel model);
        bool Edit(string ID, RentalApplicationModel model);
        bool Delete(string ID, string remarks);
        List<object> GetDropDown();
        List<object> GetRequirements(int type);
        List<object> GetTransientList(string dte);
        List<object> GetStallList(DateTime dte);
    }
    public class RentalApplicationRepository : IRentalApplicationRepository
    {
        public bool Delete(string ID, string remarks)
        {

            string sql = "update market.rental_application_status SET end_time = CURRENT_TIMESTAMP, remarks = '" + remarks + "' " +
                "       where rental_application_id = '" + ID + "' and end_time is null";
            QueryModule.Execute<int>(sql, new { tenant_profile_id = ID });

            sql = "update market.rental_application SET status = 'Cancelled' where rental_application_id = '" + ID + "' ";
            QueryModule.Execute<int>(sql, new { tenant_profile_id = ID });

            RentalApplicationStatus status = new RentalApplicationStatus();
            status.rental_application_id = ID;
            status.status_id = 0;
            status.activity = "Cancelled";
            status.user_id = GlobalObject.user_id;

            sql = "insert into market.rental_application_status (" + ObjectSqlMapping.MapInsert<RentalApplicationStatus>() + ")";
            QueryModule.Execute<int>(sql, status);

            return true;
        }

        public bool Edit(string ID, RentalApplicationModel model)
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

                    model.rental_application_id = ID;
                    model.status = "Draft";
                    string sql = "update market.rental_application SET " + ObjectSqlMapping.MapUpdate<RentalApplicationModel>() + " where rental_application_id = @rental_application_id";
                    QueryModule.Execute<int>(sql, model);

                    sql = "update market.rental_application_status SET end_time = CURRENT_TIMESTAMP where rental_application_id = @rental_application_id and end_time is null";
                    QueryModule.Execute<int>(sql, new { rental_application_id = ID });


                    model.rental_application_status = new RentalApplicationStatus();
                    model.rental_application_status.rental_application_id = ID;
                    model.rental_application_status.status_id = 3;
                    model.rental_application_status.activity = "Updated";
                    model.rental_application_status.user_id = GlobalObject.user_id;

                    sql = "insert into market.rental_application_status (" + ObjectSqlMapping.MapInsert<RentalApplicationStatus>() + ")";
                    QueryModule.Execute<int>(sql, model.rental_application_status);

                    sql = "delete from market.rental_application_requirements where rental_application_id = '" + ID + "'    ";
                    QueryModule.Execute<int>(sql);

                    foreach (RentalApplicationRequirements req in model.reqs)
                    {
                        req.rental_application_id = model.rental_application_id;
                        sql = "insert into market.rental_application_requirements (" + ObjectSqlMapping.MapInsert<RentalApplicationRequirements>() + ")";
                        QueryModule.Execute<int>(sql, req);
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

            public RentalApplicationModel GetByID(string ID)
            {
            string sql = "select * from market.rental_application app where app.rental_application_id = '" + ID + "'";
            RentalApplicationModel model = (RentalApplicationModel)QueryModule.DataObject<RentalApplicationModel>(sql);

            sql = "select * from market.rental_application_status stat where stat.end_time is null and stat.rental_application_id = '" + ID + "'";
            model.rental_application_status = (RentalApplicationStatus)QueryModule.DataObject<RentalApplicationStatus>(sql);


            sql = @"select * from market.rental_application_requirements reqs where 
                        
                    reqs.rental_application_id = '" + ID + "'  ";
            model.reqs = (List<RentalApplicationRequirements>)QueryModule.DataSource<RentalApplicationRequirements>(sql);



            return model;
            }

        public List<object> GetDropDown()
        {

            string sql = "select * from market.rental_application_type";
            List<object> list = (List<object>)QueryModule.DataSource<object>(sql);

            return list;
        }

        public List<object> GetList(int? status_id = 0, string? type = "")
        {
            string typeFilter = "";
            //if (type != "" && type != null)
            //{
            //    typeFilter = "AND app.application_type_id = '" + type + "'";   
            //}

            if (status_id != 0)
            {
                //typeFilter = typeFilter + " AND stat.status_id = " + status_id;
                typeFilter = " AND stat.status_id = " + status_id;
            }

            string sql = @"select app.*, type.name as `type` from  market.rental_application app
                inner join market.rental_application_status stat on stat.rental_application_id = app.rental_application_id
                inner join market.rental_application_type type on type.application_type_id = app.application_type_id
                WHERE stat.end_time is NULL " + typeFilter + " ORDER BY app.id ASC";

            List<object> data = (List<object>)QueryModule.DataSource<object>(sql);


            ExpandoObject form = Forms.getForm("rental-application");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT rental.applicant_name as q, 'an' as tag
                    FROM market.`rental_application` rental

                    UNION

                    SELECT
                    rental2.nature_of_business as q, 'nob' as tag
                    FROM market.`rental_application` rental2

                    UNION

                    SELECT
                    rental3.address as q, 'ad' as tag
                    FROM market.`rental_application` rental3

                    UNION

                    SELECT
                    market.`rental_application_type`.`name` as q, 'type' as tag
                    FROM market.`rental_application` rental4
                    INNER JOIN market.`rental_application_type` ON market.`rental_application_type`.application_type_id = rental4.application_type_id
                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);


            List<object> list = new List<object>();
            list.Add(data);
            list.Add(form);
            list.Add(searches);
            return list;
        }

        public List<object> GetRequirements(int type)
        {
            var typeString = "";

            if (type == 1)
            {
                typeString = "stall";
            }
            else
            {
                typeString = "transient";
            }

            string sql = @"SELECT
	                            *
                            FROM
	                            market.requirements req
                            WHERE
	                            req.application_type = 'both'
                            OR req.application_type = '" + typeString + @"'
                            ORDER BY
	                            req.order ASC";

            List<object> list = (List<object>)QueryModule.DataSource<object>(sql);

            return list;
        }

        public List<object> GetStallList(DateTime dte)
        {
            string sql = @"select sec.section_name, 
			
			            CASE WHEN app.applicant_name is null THEN
				            'vacant' ELSE
			            app.applicant_name END as `applicant`,
			            prop.property_name,
			            app.nature_of_business,
			            '' as remarks

                        from market.property prop

                        left join market.tenant_profile_dtl tntDtl on tntDtl.property_id = prop.property_id and tntDtl.end_time is null
                        left join market.tenant_profile tnt on tnt.tenant_profile_id = tntDtl.main_id
                        left join market.property_section propSec on prop.property_id = propSec.property_id
                        left join market.section sec on sec.section_id = propSec.section_id
                        left join market.rental_application app on app.applicant_id = tnt.tenant_id and app.application_type_id = 1
                        left join market.rental_applicatioN_status appstat on appstat.rental_application_id = app.rental_application_id
                        and DATE(appstat.start_time) <= '" + dte + @"'
                        where sec.section_name <> '' and prop.property_type_id = 1 
                        order by sec.section_name asc";
            List<object> record = (List<object>)QueryModule.DataSource<object>(sql);


            ExpandoObject form = Forms.getForm("rental-application");

            List<object> list = new List<object>();
            list.Add(record);
            list.Add(form);

            return list;
        }

        public List<object> GetTransientList(string dte)
        {
            string sql = @"SELECT
	                            app.*, stat.start_time
                            FROM
	                            market.`rental_application` app
                            INNER JOIN market.rental_application_status stat ON stat.rental_application_id = app.rental_application_id
                            AND stat.end_time IS NULL
                            AND stat.status_id = 5
                            WHERE
	                            DATE(stat.start_time) <= DATE('" + dte + "')";
            List<object> record = (List<object>)QueryModule.DataSource<object>(sql);


            ExpandoObject form = Forms.getForm("rental-application");

            List<object> list = new List<object>();
            list.Add(record);
            list.Add(form);

            return list;
        }

        public bool Insert(RentalApplicationModel model)
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
                    model.form_trans_no = FormNumberGenerator.generateFormNumber("rental-application");
                    model.rental_application_id = Guid.NewGuid().ToString();
                    model.status = "Draft";
                    string sql = "insert into market.rental_application (" + ObjectSqlMapping.MapInsert<RentalApplicationModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    model.rental_application_status = new RentalApplicationStatus();
                    model.rental_application_status.rental_application_id = model.rental_application_id;
                    model.rental_application_status.activity = "Added";
                    model.rental_application_status.status_id = 3;

                    sql = "insert into market.rental_application_status (" + ObjectSqlMapping.MapInsert<RentalApplicationStatus>() + ")";
                    QueryModule.Execute<int>(sql, model.rental_application_status);

                    foreach(RentalApplicationRequirements req in model.reqs)
                    {
                        req.rental_application_id = model.rental_application_id;
                        sql = "insert into market.rental_application_requirements (" + ObjectSqlMapping.MapInsert<RentalApplicationRequirements>() + ")";
                        QueryModule.Execute<int>(sql, req);
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
