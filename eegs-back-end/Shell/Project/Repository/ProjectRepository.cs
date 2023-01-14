using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using eegs_back_end.Shell.Project.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Shell.Project.Repository
{
    public interface IProjectRepository : IGlobalInterface
    {
        ProjectModel GetByID(string ID);
        bool Insert(ProjectModel model);
        bool Edit(string GUID, ProjectModel model);
        List<object> GetList();
        List<object> GetConfigValues();
        bool Delete(string ID);
    }
    public class ProjectRepository : IProjectRepository
    {
        public bool Delete(string GUID)
        {
            string sql = "UPDATE general.project_title SET status = 'Deleted' where general.project_title.project_title_guid = @project_title_guid";
            if ((int)QueryModule.Execute<int>(sql, new { project_title_guid = GUID }) == 1)
                return true;
            return false;
        }
        public bool Edit(string GUID, ProjectModel model)
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

                    model.project_title_guid = GUID;
                    var sql = "UPDATE general.project_title SET " + ObjectSqlMapping.MapUpdate<ProjectModel>() + " WHERE general.project_title.project_title_guid = @project_title_guid ";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    model.lgu_city_mun_config.project_title_guid = model.project_title_guid;
                    sql = "UPDATE general_address.lgu_city_mun_config SET " + ObjectSqlMapping.MapUpdate<LGUCityMunConfig>() + " WHERE general_address.lgu_city_mun_config.project_title_guid = @project_title_guid ";
                    QueryModule.Execute<int>(sql, model.lgu_city_mun_config);

                    model.report_config.project_title_guid = model.project_title_guid;
                    sql = "UPDATE general.tbl_general_header_setup SET " + ObjectSqlMapping.MapUpdate<LGUReportConfiguration>() + " WHERE general.tbl_general_header_setup.project_title_guid = @project_title_guid ";
                    QueryModule.Execute<int>(sql, model.report_config);


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

        public ProjectModel GetByID(string ID)
        {
            string sql = "select * from general.project_title where project_title_guid = @id";
            ProjectModel model = (ProjectModel)QueryModule.DataObject<ProjectModel>(sql, new {id = ID });

            sql = "select * from general.tbl_general_header_setup where project_title_guid = @id";
            model.report_config = (LGUReportConfiguration)QueryModule.DataObject<LGUReportConfiguration>(sql, new { id = ID });

            sql = "select * from general_address.lgu_city_mun_config where project_title_guid = @id";
            model.lgu_city_mun_config = (LGUCityMunConfig)QueryModule.DataObject<LGUCityMunConfig>(sql, new { id = ID });

            return model;
        }

        public List<object> GetConfigValues()
        {
            string sql = "select * from general_address.lgu_region_setup_temp order by reg_name asc";
            List<object> region = (List<object>)QueryModule.DataSource<object>(sql);

            sql = "select * from general_address.lgu_province_setup_temp order by province_name asc";
            List<object> province = (List<object>)QueryModule.DataSource<object>(sql);

            sql = "select * from general_address.lgu_city_mun_setup_temp order by city_mun_name asc";
            List<object> citmun = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();

            list.Add(new { region = region });
            list.Add(new { province = province });
            list.Add(new { citmun = citmun });

            return list;
        }

        public List<object> GetList()
        {
            string sql = @"select project_title.project_title_guid, project_title.savedate,project_title.title_name, project_title.description, project_title.tel_no,  project_title.`status`	
                        from general.project_title
                        inner join general.tbl_general_header_setup header on header.project_title_guid = project_title.project_title_guid
                        inner join general_address.lgu_city_mun_config lgu on lgu.project_title_guid = project_title.project_title_guid
                        where project_title.`status` = 'Active'";
            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            return obj;
        }

        public bool Insert(ProjectModel model)
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
                    model.project_title_guid = Guid.NewGuid().ToString();
                    model.status = "Active";
                    string sql = "insert into general.project_title (" + ObjectSqlMapping.MapInsert<ProjectModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    model.lgu_city_mun_config.project_title_guid = model.project_title_guid;
                    sql = "insert into general_address.lgu_city_mun_config (" + ObjectSqlMapping.MapInsert<LGUCityMunConfig>() + ")";
                   QueryModule.Execute<int>(sql, model.lgu_city_mun_config);

                    model.report_config.project_title_guid = model.project_title_guid;
                    sql = "insert into general.tbl_general_header_setup (" + ObjectSqlMapping.MapInsert<LGUReportConfiguration>() + ")";
                   QueryModule.Execute<int>(sql, model.report_config);

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
