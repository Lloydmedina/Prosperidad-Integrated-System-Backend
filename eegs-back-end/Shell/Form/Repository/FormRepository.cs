using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using eegs_back_end.Shell.Form.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace eegs_back_end.Shell.Form.Repository
{
    public interface IFormRepository : IGlobalInterface
    {

        
        List<object> GetList();
        bool Edit(string GUID, FormModel model);
        bool Insert(FormModel model);
        List<object> GetDomainFolders(string ID);
        FormModel GetByID(string GUID);
        List<object> GetFilterOptions();
        List<object> GetActionTypes();
    }
    public class FormRepository : IFormRepository
    {
        public bool Edit(string GUID, FormModel model)
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
                    model.form_guid = GUID;
                    var sql = "UPDATE general.tbl_user_form SET " + ObjectSqlMapping.MapUpdate<FormModel>() + " WHERE general.tbl_user_form.form_guid = '" + GUID + "'   ";
                    int res = (int)QueryModule.Execute<int>(sql, model);


                    sql = "DELETE FROM general.activity WHERE general.activity.form_guid = '" + GUID + "'   ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM general.tbl_form_setup_series WHERE general.tbl_form_setup_series.form_guid = '" + GUID + "'   ";
                    QueryModule.Execute<int>(sql, model);


                    if (model.routes != null && model.routes.Count > 0)
                    {
                        foreach (Activity obj in model.routes)
                        {
                            obj.form_guid = model.form_guid;
                            obj.activity_guid = Guid.NewGuid().ToString();
                            sql = "INSERT INTO general.activity (" + ObjectSqlMapping.MapInsert<Activity>() + ")";
                            QueryModule.Execute<int>(sql, obj);
                        }
                    }


                    if (model.form_series != null)
                    {
                        foreach (FormSeries series in model.form_series)
                        {
                            series.form_guid = model.form_guid;
                            sql = "INSERT INTO general.tbl_form_setup_series (" + ObjectSqlMapping.MapInsert<FormSeries>() + ")";
                            QueryModule.Execute<int>(sql, series);
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

        public List<object> GetActionTypes()
        {
            string sql = "select * from general.action_type";
            List<object> list = (List<object>)QueryModule.DataSource<object>(sql);

            return list;
        }

        public FormModel GetByID(string GUID)
        {
            string sql = @"select * from general.tbl_user_form where tbl_user_form.form_guid = @form_guid";
            FormModel obj = (FormModel)QueryModule.DataObject<FormModel>(sql, new { form_guid = GUID });

            if (obj.form_type == "Form")
            {
                sql = @"select * from general.tbl_form_setup_series where  tbl_form_setup_series.form_guid = @form_guid";
                obj.form_series = (List<FormSeries>)QueryModule.DataSource<FormSeries>(sql, new { form_guid = GUID });


                sql = @"select * from general.activity where activity.form_guid = @form_guid";
                obj.routes = (List<Activity>)QueryModule.DataSource<Activity>(sql, new { form_guid = GUID });

            }
            return obj;
        }

        public List<object> GetDomainFolders(string ID)
        {
            string sql = @"select tbl_user_form.form_guid, tbl_user_form.form_name from general.tbl_user_form 
                            where tbl_user_form.form_type = 'Folder' AND tbl_user_form.domain_guid = @domain_guid";
            List<object> list = (List<object>)QueryModule.DataSource<object>(sql, new { domain_guid = ID });

            return list;
        }

        public List<object> GetFilterOptions()
        {
            string sql = "select * from general.form_filter";
            List<object> list = (List<object>)QueryModule.DataSource<object>(sql);

            return list;
        }

        public List<object> GetList()
        {
            string sql = @"SELECT	form_guid, domain.domain_guid, form_name, domain_name, form_type, form_status  FROM general.`tbl_user_form` 
                            inner join general.domain on general.domain.domain_guid = general.tbl_user_form.domain_guid
                            ORDER BY domain_name, form_name";
           List<object> list = (List<object>)QueryModule.DataSource<object>(sql);

            return list;
        }

        public bool Insert(FormModel model)
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
                    model.form_guid = Guid.NewGuid().ToString();
                    model.form_status = "Active";
                    var sql = "INSERT INTO general.tbl_user_form (" + ObjectSqlMapping.MapInsert<FormModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    if (model.routes != null && model.routes.Count > 0)
                    {
                        foreach (Activity obj in model.routes)
                        {
                            obj.form_guid = model.form_guid;
                            obj.activity_guid = Guid.NewGuid().ToString();
                            sql = "INSERT INTO general.activity (" + ObjectSqlMapping.MapInsert<Activity>() + ")";
                            QueryModule.Execute<int>(sql, obj);
                        }
                    }


                    if(model.form_series != null)
                    {
                        foreach (FormSeries series in model.form_series)
                        {
                            series.form_guid = model.form_guid;
                            sql = "INSERT INTO general.tbl_form_setup_series (" + ObjectSqlMapping.MapInsert<FormSeries>() + ")";
                            QueryModule.Execute<int>(sql, series);
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
