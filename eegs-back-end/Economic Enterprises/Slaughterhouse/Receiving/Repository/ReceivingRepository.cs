using eegs_back_end.DbModule;
using eegs_back_end.Economic_Enterprises.Slaughterhouse.Receiving.Model;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Economic_Enterprises.Slaughterhouse.Receiving.Repository
{
    public interface IReceivingRepository : IGlobalInterface
    {
        List<object> GetList(int? status_id = 0);
        ReceivingModel GetByID(string ID);
        bool Insert(ReceivingModel model);
        bool Edit(string ID, ReceivingModel model);
        bool Delete(string ID);
        bool Inspect(AnimalInspection model);
        List<object> GetDropDown();
        List<object> GetReceivedList();
        List<object> GetPrintByID(string ID);
    }
    public class ReceivingRepository : IReceivingRepository
    {
        public bool Delete(string ID)
        {
            string sql = "update slaughterhouse.main SET status = 'Cancelled' where  main_id = '" + ID + "'";
            QueryModule.Execute<int>(sql);


            return true;
        }

        public bool Edit(string ID, ReceivingModel model)
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
                    string sql = "update slaughterhouse.main SET " + ObjectSqlMapping.MapUpdate<ReceivingModel>() + " where main_id = @main_id";
                    QueryModule.Execute<int>(sql, model);


                    sql = "DELETE FROM slaughterhouse.receiving where main_id = '" + ID + "' and status = 'Draft' ";
                    QueryModule.Execute<int>(sql, model);


                    if (model.receiving_list != null && model.receiving_list.Count > 0)
                    {
                        foreach (ReceivingObj obj in model.receiving_list)
                        {
                            if (obj.status == "Draft")
                            {
                                obj.receiving_id = "";
                            }
                            obj.main_id = model.main_id;
                            obj.time = Convert.ToDateTime(obj.time.ToShortTimeString()).ToLocalTime();
                            obj.slaughtering_date = Convert.ToDateTime(obj.slaughtering_date.ToShortDateString());
                            obj.slaughtering_time = Convert.ToDateTime(obj.slaughtering_time.ToShortTimeString()).ToLocalTime();
                            if (obj.receiving_id == "")
                            {
                                obj.receiving_id = Guid.NewGuid().ToString();
                                sql = "insert into slaughterhouse.receiving (" + ObjectSqlMapping.MapInsert<ReceivingObj>() + ")";
                                QueryModule.Execute<int>(sql, obj);
                            }
                            else
                            {
                                    sql = "update slaughterhouse.receiving SET " + ObjectSqlMapping.MapUpdate<ReceivingObj>() + " where receiving_id = @receiving_id";
                                    QueryModule.Execute<int>(sql, obj);
                            }

                            sql = "update slaughterhouse.animal_inspection SET end_time = CURRENT_TIMESTAMP where receiving_id = '" + obj.receiving_id + "' and end_time is null";
                            QueryModule.Execute<int>(sql, obj);

                            if (obj.status == "Inspected")
                            {
                                obj.animal_inspection.receiving_id = obj.receiving_id;
                                sql = "insert into slaughterhouse.animal_inspection (" + ObjectSqlMapping.MapInsert<AnimalInspection>() + ")";
                                QueryModule.Execute<int>(sql, obj.animal_inspection);
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

        public ReceivingModel GetByID(string ID)
        {
            string sql = @"select * from slaughterhouse.main where main_id = '" + ID + "'   ";
            ReceivingModel obj = (ReceivingModel)QueryModule.DataObject<ReceivingModel>(sql);

            sql = "select * from slaughterhouse.receiving where main_id = '" + ID + "' ";
            obj.receiving_list = (List<ReceivingObj>)QueryModule.DataSource<ReceivingObj>(sql);

            foreach(ReceivingObj rcv in obj.receiving_list)
            {
                rcv.animal_inspection = new AnimalInspection();
                sql = "select * from slaughterhouse.animal_inspection where receiving_id = '" + rcv.receiving_id + "' and end_time is null ";
                rcv.animal_inspection = (AnimalInspection)QueryModule.DataObject<AnimalInspection>(sql);
            }


            return obj;
        }

        public List<object> GetDropDown()
        {
            string sql = @"select * from slaughterhouse.animal";
            List<object> list = (List<object>)QueryModule.DataSource<object>(sql);

            return list;
        }

        public List<object> GetList(int? status_id = 0)
        {
            string sql = "select * from slaughterhouse.main";
            List<object> list = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("receiving");


            List<object> listObj = new List<object>();
            listObj.Add(list);
            listObj.Add(form);


            return listObj;

        }

        public List<object> GetPrintByID(string ID)
        {
            string sql = @"select main.transaction_date, rec.*, animal.name as `animal` from slaughterhouse.receiving rec
                        inner join slaughterhouse.main main on main.main_id = rec.main_id
                        inner join slaughterhouse.animal animal on animal.animal_id = rec.animal_id
                        where main.main_id = '" + ID + "' ";
            List<object> list = (List<object>)QueryModule.DataSource<object>(sql);

            return list;
        }
        public List<object> GetReceivedList()
        {
            string sql = @"select main.transaction_date, rec.*, animal.name as `animal` from slaughterhouse.receiving rec
                        inner join slaughterhouse.main main on main.main_id = rec.main_id
                        inner join slaughterhouse.animal animal on animal.animal_id = rec.animal_id
                        where rec.status = 'Inspected' ";
            List<object> list = (List<object>)QueryModule.DataSource<object>(sql);

            return list;
        }

        public bool Insert(ReceivingModel model)
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

                    model.main_id = Guid.NewGuid().ToString();
                    model.status = "Active";
                    string sql = "insert into slaughterhouse.main (" + ObjectSqlMapping.MapInsert<ReceivingModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    
                    if(model.receiving_list != null && model.receiving_list.Count > 0)
                    {
                        foreach(ReceivingObj obj in model.receiving_list)
                        {
                            obj.receiving_id = Guid.NewGuid().ToString();
                            obj.main_id = model.main_id;
                            obj.time = Convert.ToDateTime(obj.time.ToShortTimeString()).ToLocalTime();
                            obj.slaughtering_date = Convert.ToDateTime(obj.slaughtering_date.ToShortDateString());
                            obj.slaughtering_time = Convert.ToDateTime(obj.slaughtering_time.ToShortTimeString()).ToLocalTime();
                            sql = "insert into slaughterhouse.receiving (" + ObjectSqlMapping.MapInsert<ReceivingObj>() + ")";
                            res = (int)QueryModule.Execute<int>(sql, obj);

                            if (obj.status == "Inspected")
                            {
                                obj.animal_inspection.receiving_id = obj.receiving_id;
                                sql = "insert into slaughterhouse.animal_inspection (" + ObjectSqlMapping.MapInsert<AnimalInspection>() + ")";
                                QueryModule.Execute<int>(sql, obj.animal_inspection);
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

        public bool Inspect(AnimalInspection model)
        {

           string sql = "insert into slaughterhouse.animal_inspection (" + ObjectSqlMapping.MapInsert<AnimalInspection>() + ")";
            QueryModule.Execute<int>(sql, model);

            sql = "update slaughterhouse.receiving SET status = 'Inspected' where receiving_id = '" + model.receiving_id + "'  ";
            QueryModule.Execute<int>(sql);


            return true;
        }
    }
}
