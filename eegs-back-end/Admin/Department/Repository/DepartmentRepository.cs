using eegs_back_end.Admin.Department.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.Department.Repository
{
    public interface IDepartmentRepository: IGlobalInterface
    {
        List<object> GetList();
        List<object> GetEmployees();
        string GetPrefix(string domID);
        DepartmentModel GetByID(string ID);
        bool Insert(DepartmentModel model);
        bool Edit(string ID, DepartmentModel model);
        bool Delete(string ID);
    }
    public class DepartmentRepository : IDepartmentRepository
    {
        public bool Delete(string ID)
        {
           string sql = "update humanresource.department SET status = 'Deleted' where dept_id = @dept_id";
           int i = (int)QueryModule.Execute<int>(sql, new {dept_id = ID });
            if (i == 0) return false;

            return true;
        }

        public bool Edit(string ID, DepartmentModel model)
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
                    model.dept_id = ID;
                    model.status = "Active";

                    string sql = "update humanresource.department SET " + ObjectSqlMapping.MapUpdate<DepartmentModel>() + " WHERE department.dept_id = @dept_id";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    sql = "update humanresource.department_head SET end_time = CURRENT_TIMESTAMP where dept_id = @dept_id and end_time is null";
                    QueryModule.Execute<int>(sql, model);


                    model.dept_head.dept_id = model.dept_id;
                    model.dept_head.status = "Active";
                    model.dept_head.pk_id = Guid.NewGuid().ToString();
                    sql = "insert into humanresource.department_head (" + ObjectSqlMapping.MapInsert<DepartmentHead>() + ")";
                    QueryModule.Execute<int>(sql, model.dept_head);

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

        public DepartmentModel GetByID(string ID)
        {
            string sql = @"select * from humanresource.department dept where dept.dept_id = @dept_id";
            DepartmentModel model = (DepartmentModel)QueryModule.DataObject<DepartmentModel>(sql, new { dept_id = ID });

            sql = @"select * from humanresource.department_head dept_head where dept_head.dept_id = @dept_id and dept_head.end_time is null";
            model.dept_head = (DepartmentHead)QueryModule.DataObject<DepartmentHead>(sql, new { dept_id = ID });

            return model;
        }

        public List<object> GetEmployees()
        {
            string sql = @"select employee_id, employee_name from humanresource.employees";
            List<object> list = (List<object>)QueryModule.DataSource<object>(sql);

            return list;
        }

        public List<object> GetList()
        {
            string sql = @"
                select dept.dept_id, dept.dept_name, dept.short_desc, dept.dept_code, head_emp.employee_name as dept_head, date_save, dept.status
                from humanresource.department dept
                left join humanresource.department_head dept_head on dept_head.dept_id = dept.dept_id and dept_head.end_time is null
				left join humanresource.employees head_emp on head_emp.employee_id = dept_head.head_employee_id
				left join humanresource.employees asst_emp on asst_emp.employee_id = dept_head.asst_employee_id
                        ";
            List<object> dept = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("department");

            List<object> list = new List<object>();
            list.Add(dept);
            list.Add(form);
            return list;
        }

        public string GetPrefix(string domID)
        {
            string sql = @"SELECT gen.status_name as prefix FROM general_address.`lgu_city_mun_config` gen
                    inner join general.domain dom on dom.project_title_guid = gen.project_title_guid
                    where dom.domain_guid = '"+ domID +"'";
            string prefix = (string)QueryModule.DataObject<string>(sql);

            return prefix;
        }

        public bool Insert(DepartmentModel model)
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
                    model.dept_id = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = "insert into humanresource.department (" + ObjectSqlMapping.MapInsert<DepartmentModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);


                    model.dept_head.dept_id = model.dept_id;
                    model.dept_head.status = "Active";
                    model.dept_head.pk_id = Guid.NewGuid().ToString();
                    sql = "insert into humanresource.department_head (" + ObjectSqlMapping.MapInsert<DepartmentHead>() + ")";
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
    }
}
