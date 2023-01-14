using eegs_back_end.DbModule;
using eegs_back_end.Economic_Enterprises.Slaughterhouse.SlaughterhouseRecords.Model;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Economic_Enterprises.Slaughterhouse.SlaughterhouseRecords.Repository
{
    public interface ISlaughterhouseRecordsRepository: IGlobalInterface
    {
        List<object> GetList(int? status_id = 0);
        List<object> GetPrint(int monthOf);
        object GetByID(string ID);
        bool Insert(SlaughterhouseRecordsModel model);
        bool Edit(string ID, SlaughterhouseRecordsModel model);
        bool Delete(string ID);
    }
    public class SlaughterhouseRecordsRepository : ISlaughterhouseRecordsRepository
    {
        public bool Delete(string ID)
        {
            string sql = "update slaughterhouse.SlaughterhouseRecords SET status = 'Cancelled' where SlaughterhouseRecords_id = '" + ID + "'";
            QueryModule.Execute<int>(sql);

            return true;
        }

        public bool Edit(string ID, SlaughterhouseRecordsModel model)
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

                    string sql = "update slaughterhouse.SlaughterhouseRecords SET " + ObjectSqlMapping.MapUpdate<SlaughterhouseRecordsModel>() + " where SlaughterhouseRecords_id = @SlaughterhouseRecords_id";
                    QueryModule.Execute<int>(sql, model);

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

        public object GetByID(string ID)
        {
            string sql = @"select slaughter.*, rec.client_name from slaughterhouse.SlaughterhouseRecords slaughter
                        inner join slaughterhouse.receiving rec on rec.receiving_id = slaughter.receiving_id
                        where slaughter.SlaughterhouseRecords_id = '" + ID + "' ";
            object data = (object)QueryModule.DataObject<object>(sql);

            return data;
        }

        public List<object> GetList(int? status_id = 0)
        {
            string sql = @"SELECT slaughterhouse.slaughterhouse_records.*
                            FROM slaughterhouse.slaughterhouse_records
                            WHERE slaughterhouse.slaughterhouse_records.status = 'Active' ";
            List<object> data = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject list = Forms.getForm("slaughterhouse-records");

            List<object> newList = new List<object>();

            newList.Add(data);
            newList.Add(list);

            return newList;
        }

        public List<object> GetPrint(int monthOf)
        {
            string sql = @"select slaughter.*, rec.client_name, rec.kilos, rec.purpose, animal.name as `animal` from slaughterhouse.SlaughterhouseRecords slaughter
                        inner join slaughterhouse.receiving rec on rec.receiving_id = slaughter.receiving_id
                        inner join slaughterhouse.animal animal on animal.animal_id = rec.animal_id
                        
                        where MONTH(slaughter.date_slaughtered) = " + monthOf + " ";
            List<object> data = (List<object>)QueryModule.DataSource<object>(sql);

            return data;

        }

        public bool Insert(SlaughterhouseRecordsModel model)
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
                    model.slaughterhouse_records_id = Guid.NewGuid().ToString();
                    model.status = "Active";
                    model.date_slaughtered = Convert.ToDateTime(model.date_slaughtered.ToShortDateString()).ToLocalTime();
                    string sql = "insert into slaughterhouse.SlaughterhouseRecords (" + ObjectSqlMapping.MapInsert<SlaughterhouseRecordsModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);


                    sql = "update slaughterhouse.receiving SET status = 'Slaughtered' where receiving_id = '" + model.receiving_id + "'";
                    QueryModule.Execute<int>(sql);


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
