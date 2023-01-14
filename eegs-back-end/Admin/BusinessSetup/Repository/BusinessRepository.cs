using eegs_back_end.Admin.BusinessSetup.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace eegs_back_end.Admin.BusinessSetup.Repository
{
    public interface IBusinessRepository : IGlobalInterface
    {
        List<object> GetList(int? status_id = 0,int? reg_status = 0);
        BusinessModel GetByID(string ID);
        bool Insert(BusinessModel model);
        bool Edit(string ID, BusinessModel model);
        bool Delete(string ID);
        List<object> GetBusinessEntity();
        List<object> GetHistory(string ID);
        public class BusinessRepository : FormNumberGenerator, IBusinessRepository
        {
            public bool Delete(string ID)
            {
                BusinessStatus status = new BusinessStatus();
                string sql = "update general.business_status SET end_time = CURRENT_TIMESTAMP where business_id = @business_id and end_time is null";
                QueryModule.Execute<int>(sql, new { business_id = ID });

                status.business_id = ID;
                status.status_id = 2;
                status.activity = "Deleted";
                status.user_id = GlobalObject.user_id;

                sql = "insert into general.business_status (" + ObjectSqlMapping.MapInsert<BusinessStatus>() + ")";
                QueryModule.Execute<int>(sql, status);

                return true;
            }

            public bool Edit(string ID, BusinessModel model)
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
                        model.business_id = ID;
                        string sql = "update general.business SET " + ObjectSqlMapping.MapUpdate<BusinessModel>() + " where business_id = @business_id";
                        QueryModule.Execute<int>(sql, model);

                        sql = "update general.business_status SET end_time = CURRENT_TIMESTAMP where business_id = @business_id and end_time is null";
                        QueryModule.Execute<int>(sql, new { business_id = ID });


                        model.status = new BusinessStatus();
                        model.status.business_id = ID;
                        model.status.status_id = 1;
                        model.status.prev_record = JsonSerializer.Serialize(GetByID(ID));
                        model.status.activity = "Updated";
                        model.status.user_id = GlobalObject.user_id;



                        sql = "insert into general.business_status (" + ObjectSqlMapping.MapInsert<BusinessStatus>() + ")";
                        QueryModule.Execute<int>(sql, model.status);

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

            public BusinessModel GetByID(string ID)
            {
                string sql = @"select * from general.business where business_id = @business_id ";
                BusinessModel model = (BusinessModel)QueryModule.DataObject<BusinessModel>(sql, new { business_id = ID });

                sql = @"select * from general.business_status where business_id = @business_id and end_time is null";
                model.status = (BusinessStatus)QueryModule.DataObject<BusinessStatus>(sql, new { business_id = ID });

                return model;
            }

            public List<object> GetBusinessEntity()
            {
                string sql = "SELECT * FROM general.business_entity";
                List<object> business_entity = (List<object>)QueryModule.DataSource<object>(sql);

                return business_entity;
            }

            public List<object> GetHistory(string ID)
            {
                //string sql = @"SELECT h.*, user.UserFull_Name as `user`  FROM general.`business_status` h 
                //            inner join general.users user on user.user_guid = h.user_id
                //            where h.business_id = 'a13d085a-5002-4fdc-bc83-7ca0e20aae44' 
                //            and end_time is not NULL
                //            and prev_record is not NULL;";
                string sql = @"SELECT h.*, user.UserFull_Name as `user`  FROM general.`business_status` h 
                inner join general.users user on user.user_guid = h.user_id
                where h.business_id = @id
                order by h.id desc limit 6
                ";
                List<object> list = (List<object>)QueryModule.DataSource<object>(sql, new {id = ID });
                return list;
            }

            public List<object> GetList(int? status_id = 0, int? reg_status = 0)
            {
                string filter = "";
                if(status_id != 0)
                {
                    filter = "and bstat.status_id = " + status_id;
                }
                string sql = @"
                          
                          
                          select bstat.start_time as savedate, biz.form_trans_no, biz.business_id, biz.business_name,  biz.trade_name, biz.owner_name, biz.tel_no,
                        biz.street, biz.building, 
                        (select brgy_name from general_address.lgu_brgy_setup_temp where brgy_id = biz.brgy_id ) as brgy,
                        stat.status, biz.owner_name, biz.brgy_id

                        from general.business biz
                        inner join general.business_status bstat on bstat.business_id = biz.business_id and bstat.end_time is null
                        " + filter + @"
                        inner join general.transaction_status stat on stat.status_id = bstat.status_id
                        where biz.reg_status = " + reg_status + @"
                        order by biz.business_name ASC
                        ";
                List<object> data = (List<object>)QueryModule.DataSource<object>(sql);


                ExpandoObject form = Forms.getForm("business");

                sql = @"SELECT z.q as val, z.tag  FROM (

                        SELECT biz.business_name as q, 'bn' as tag
                        FROM general.`business` biz
                        UNION

                        SELECT
                        biz2.trade_name as q,
                        'tn' as tag
                        FROM general.`business` biz2

                        UNION

                        SELECT
                        brgy.brgy_name  as q,
                        'br' as tag
                        FROM general.`business` biz3
                        inner join general_address.lgu_brgy_setup_temp brgy on brgy.brgy_id = biz3.brgy_id) z";
                List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);


                List<object> list = new List<object>();
                list.Add(data);
                list.Add(form);
                list.Add(searches);
                return list;
            }

            public bool Insert(BusinessModel model)
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
                        model.form_trans_no = generateFormNumber("business");
                        model.business_id = Guid.NewGuid().ToString();
                        model.entity_id = model.entity_id;

                        string sql = "insert into general.business (" + ObjectSqlMapping.MapInsert<BusinessModel>() + ")";
                        int res = (int)QueryModule.Execute<int>(sql, model);


                        model.status = new BusinessStatus();
                        model.status.business_id = model.business_id;
                        model.status.status_id = 1;
                        model.status.prev_record = JsonSerializer.Serialize(model);
                        model.status.activity = "Added";
                        model.status.user_id = GlobalObject.user_id;


                        sql = "insert into general.business_status (" + ObjectSqlMapping.MapInsert<BusinessStatus>() + ")";
                        QueryModule.Execute<int>(sql, model.status);


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
}
