using eegs_back_end.DbModule;
using eegs_back_end.Economic_Enterprises.Slaughterhouse.PostMortem.Model;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Economic_Enterprises.Slaughterhouse.PostMortem.Repository
{
    public interface IPostMortemRepository: IGlobalInterface
    {
        List<object> GetList(int? status_id = 0);
        List<object> GetPrint(int monthOf);
        object GetByID(string ID);
        bool Insert(PostMortemModel model);
        bool Edit(string ID, PostMortemModel model);
        bool Delete(string ID);
    }
    public class PostMortemRepository : IPostMortemRepository
    {
        public bool Delete(string ID)
        {
            string sql = "update slaughterhouse.PostMortem SET status = 'Cancelled' where PostMortem_id = '" + ID + "'";
            QueryModule.Execute<int>(sql);

            return true;
        }

        public bool Edit(string ID, PostMortemModel model)
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

                    string sql = "update slaughterhouse.PostMortem SET " + ObjectSqlMapping.MapUpdate<PostMortemModel>() + " where PostMortem_id = @PostMortem_id";
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
            string sql = @"select slaughter.*, rec.client_name from slaughterhouse.PostMortem slaughter
                        inner join slaughterhouse.receiving rec on rec.receiving_id = slaughter.receiving_id
                        where slaughter.PostMortem_id = '" + ID + "' ";
            object data = (object)QueryModule.DataObject<object>(sql);

            return data;
        }

        public List<object> GetList(int? status_id = 0)
        {
            string sql = @"SELECT slaughterhouse.post_mortem.*
                            FROM slaughterhouse.post_mortem
                            WHERE slaughterhouse.post_mortem.status = 'Active' OR slaughterhouse.post_mortem.status = 'Approved'
                            OR slaughterhouse.post_mortem.status = 'Disapproved' ";
            List<object> data = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject list = Forms.getForm("post-mortem");

            List<object> newList = new List<object>();

            newList.Add(data);
            newList.Add(list);

            return newList;
        }

        public List<object> GetPrint(int monthOf)
        {
            string sql = @"select slaughter.*, rec.client_name, rec.kilos, rec.purpose, animal.name as `animal` from slaughterhouse.PostMortem slaughter
                        inner join slaughterhouse.receiving rec on rec.receiving_id = slaughter.receiving_id
                        inner join slaughterhouse.animal animal on animal.animal_id = rec.animal_id
                        
                        where MONTH(slaughter.date_slaughtered) = " + monthOf + " ";
            List<object> data = (List<object>)QueryModule.DataSource<object>(sql);

            return data;

        }

        public bool Insert(PostMortemModel model)
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
                    model.post_mortem_id = Guid.NewGuid().ToString();
                    model.status = "Active";
                    model.date_slaughtered = Convert.ToDateTime(model.date_slaughtered.ToShortDateString()).ToLocalTime();
                    string sql = "insert into slaughterhouse.PostMortem (" + ObjectSqlMapping.MapInsert<PostMortemModel>() + ")";
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
