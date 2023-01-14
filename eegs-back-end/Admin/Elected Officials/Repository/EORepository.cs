using eegs_back_end.Admin.Elected_Officials.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.Elected_Officials.Repository
{
    public interface IEORepository: IGlobalInterface
    {
        bool Insert(EOModel model);
        bool Edit(string id, EOModel model);
        object GetList();
        List<object> GetDepartment();
        List<object> GetEmployees();
        EOModel GetByID(string id);
        object GetPrintable(string id);
        bool Delete(string id);
    }
    public class EORepository : IEORepository
    {
        public bool Delete(string id)
        {
            string sql = "UPDATE general.tbl_general_elected_officials SET status = 'Deleted' where general.tbl_general_elected_officials.eo_id = @id";
            if ((int)QueryModule.Execute<int>(sql, new { id = id }) == 1)
                return true;
            return false;
        }

        public bool Edit(string id, EOModel model)
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
                    model.eo_id = id;
                    model.status = "Active";

                    string sql = "UPDATE general.tbl_general_elected_officials SET " + ObjectSqlMapping.MapUpdate<EOModel>() + " WHERE tbl_general_elected_officials.eo_id = @id";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    sql = "delete from general.tbl_general_elected_councillors where eo_id = '" + id + "'  ";
                    QueryModule.Execute<int>(sql);

                    if (model.councillors.Count > 0)
                    {
                        foreach (EOCouncillor obj in model.councillors)
                        {
                            obj.eo_id = model.eo_id;
                            sql = "insert into general.tbl_general_elected_councillors (" + ObjectSqlMapping.MapInsert<EOCouncillor>() + ")";
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

        public EOModel GetByID(string id)
        {
            string sql = "select * from general.tbl_general_elected_officials x where x.eo_id = @id";
            EOModel model = (EOModel)QueryModule.DataObject<EOModel>(sql, new { id = id });

            sql = "select * from general.tbl_general_elected_councillors c where c.eo_id = @id";
            model.councillors = (List<EOCouncillor>)QueryModule.DataSource<EOCouncillor>(sql, new { id = id });

            return model;
        }

        public List<object> GetDepartment()
        {
            throw new NotImplementedException();
        }

        public List<object> GetEmployees()
        {
            string sql = @"SELECT person_guid, employee_name, position_name, position_desc from humanresource.employees 
                            INNER JOIN humanresource.person_position p_pos on p_pos.person_id = employees.person_guid AND p_pos.end_time is NULL
                            INNER JOIN humanresource.position pos on pos.position_id = p_pos.position_id
                            where employees.employee_type_id = 2";
            List<object> list = (List<object>)QueryModule.DataSource<object>(sql);

            return list;
        }

        public object GetList()
        {
            string sql = @"
                        SELECT 
                        a.eo_id,
                        a.savedate,
                        proj.title_name, 
                        (select employee_name from humanresource.employees x where x.person_guid = a.mayor_id) as mayor,
                        (select employee_name from humanresource.employees x where x.person_guid = a.vmayor_id) as vmayor,
                        CONCAT(a.term_from,' - ',a.term_to) as term, a.status
                        FROM general.tbl_general_elected_officials a
                        inner join humanresource.person_position perpos on perpos.person_id = a.mayor_id
                        inner join humanresource.position pos on pos.position_id = perpos.position_id
                        inner join humanresource.department dept on dept.dept_id = pos.dept_id
                        inner join general.project_title proj on proj.project_title_guid = dept.project_title_guid";

            List<object> list = (List<object>)QueryModule.DataSource<object>(sql);

            return list;
        }

        public object GetPrintable(string id)
        {

        string sql = @"select 
                    (select emp.employee_name from humanresource.employees emp
                    where emp.person_guid = gen.mayor_id) as mayor ,

                    (select position_name from humanresource.person_position ppos
                    inner join humanresource.position pos on pos.position_id = ppos.position_id
                    where ppos.person_id = gen.mayor_id) as mayor_pos,

                    (select emp.employee_name from humanresource.employees emp
                    where emp.person_guid = gen.vmayor_id) as vmayor ,

                    (select position_name from humanresource.person_position ppos
                    inner join humanresource.position pos on pos.position_id = ppos.position_id
                    where ppos.person_id = gen.vmayor_id) as vmayor_pos

                    from general.tbl_general_elected_officials gen
                    where eo_id = @id
                    ";
            EOMainPpl model = (EOMainPpl)QueryModule.DataObject<EOMainPpl>(sql, new { id = id });

            
            sql = @"select 
                    (select emp.employee_name from humanresource.employees emp
                    where emp.person_guid = gen.councillor_id) as `name` ,

                    (select position_name from humanresource.person_position ppos
                    inner join humanresource.position pos on pos.position_id = ppos.position_id
                    where ppos.person_id = gen.councillor_id) as title

                    from general.tbl_general_elected_councillors gen
                     where eo_id = @id
                    ";
            List<EOPrintCC> cc = (List<EOPrintCC>)QueryModule.DataSource<EOPrintCC>(sql, new { id = id });

            
            return new { model, cc };
        }

        public bool Insert(EOModel model)
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
                    model.eo_id = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = "insert into general.tbl_general_elected_officials (" + ObjectSqlMapping.MapInsert<EOModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    if (model.councillors.Count > 0)
                    {
                        foreach (EOCouncillor obj in model.councillors)
                        {
                            obj.eo_id = model.eo_id;
                            sql = "insert into general.tbl_general_elected_councillors (" + ObjectSqlMapping.MapInsert<EOCouncillor>() + ")";
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
    }
}
