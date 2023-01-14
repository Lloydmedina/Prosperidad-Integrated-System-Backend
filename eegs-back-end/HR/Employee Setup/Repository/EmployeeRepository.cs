using eegs_back_end.Admin.Department.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using eegs_back_end.HR.Employee_Setup.Model;
using eegs_back_end.HR.Position_Setup.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.HR.Employee_Setup.Repository
{
    public interface IEmployeeRepository: IGlobalInterface
    {
        List<object> GetList(int? status_id = 0);
        List<object> GetActiveList(int? status_id = 0);
        EmployeeModel GetByID(string ID);
        bool Insert(EmployeeModel model);
        bool Edit(string ID, EmployeeModel model);
        bool Delete(string ID);
        List<object> GetDropDown();
    }
    public class EmployeeRepository : IEmployeeRepository
    {
        public bool Delete(string ID)
        {
            string  sql = "update humanresource.employee_status SET end_time = CURRENT_TIMESTAMP where employee_id = '" + ID + "' and end_time is NULL";
            QueryModule.Execute<int>(sql);

            sql = "insert into humanresource.employee_status (`employee_id`, `status_id`) VALUES (@id, @status_id)";
            QueryModule.Execute<int>(sql, new { id = ID, status_id = 3 });

            return true;
        }

        public bool Edit(string ID, EmployeeModel model)
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
                    model.employee_id = ID;
                    string sql = "update humanresource.employees SET " + ObjectSqlMapping.MapUpdate<EmployeeModel>() + " WHERE employee_id = '" + ID + "'";
                    QueryModule.Execute<int>(sql, model);

                    sql = "update humanresource.person_position SET end_time = CURRENT_TIMESTAMP where person_id = '" + model.person_guid + "'  and end_time is NULL";
                    QueryModule.Execute<int>(sql);

                    model.person_position.status = "Updated";
                    sql = "insert into humanresource.person_position (" + ObjectSqlMapping.MapInsert<PersonPosition>() + ")";
                    QueryModule.Execute<int>(sql, model.person_position);


                    sql = "update humanresource.person_dept SET end_time = CURRENT_TIMESTAMP where person_id = '" + model.person_guid + "'  and end_time is NULL";
                    QueryModule.Execute<int>(sql);

                    model.person_dept.status = "Updated";
                    sql = "insert into humanresource.person_dept (" + ObjectSqlMapping.MapInsert<PersonDepartment>() + ")";
                    QueryModule.Execute<int>(sql, model.person_dept);



                    sql = "update humanresource.person_payroll_position SET end_time = CURRENT_TIMESTAMP where person_id = '" + model.person_guid + "'  and end_time is NULL";
                    QueryModule.Execute<int>(sql);

                    model.person_position.status = "Updated";
                    sql = "insert into humanresource.person_payroll_position (" + ObjectSqlMapping.MapInsert<PersonPosition>() + ")";
                    QueryModule.Execute<int>(sql, model.person_payroll_position);


                    sql = "update humanresource.person_payroll_dept SET end_time = CURRENT_TIMESTAMP where person_id = '" + model.person_guid + "'  and end_time is NULL";
                    QueryModule.Execute<int>(sql);

                    model.person_dept.status = "Updated";
                    sql = "insert into humanresource.person_payroll_dept (" + ObjectSqlMapping.MapInsert<PersonDepartment>() + ")";
                    QueryModule.Execute<int>(sql, model.person_payroll_dept);

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

        public EmployeeModel GetByID(string ID)
        {
            string sql = @"select * from humanresource.employees where employee_id = '" + ID + "'";
            EmployeeModel employee = (EmployeeModel)QueryModule.DataObject<EmployeeModel>(sql);

            sql = "select * from humanresource.person_position where person_id = '" + employee.person_guid + "' and end_time is null";
            employee.person_position = (PersonPosition)QueryModule.DataObject<PersonPosition>(sql);

            sql = "select * from humanresource.person_dept where person_id = '" + employee.person_guid + "' and end_time is null";
            employee.person_dept = (PersonDepartment)QueryModule.DataObject<PersonDepartment>(sql);

            sql = "select * from humanresource.person_payroll_position where person_id = '" + employee.person_guid + "' and end_time is null";
            employee.person_payroll_position = (PersonPosition)QueryModule.DataObject<PersonPosition>(sql);

            sql = "select * from humanresource.person_payroll_dept where person_id = '" + employee.person_guid + "' and end_time is null";
            employee.person_payroll_dept = (PersonDepartment)QueryModule.DataObject<PersonDepartment>(sql);

            return employee;
        }

        class DeptListModel : DepartmentModel
        {
           public List<PositionModel> positions { get; set; }
        }


        public List<object> GetDropDown()
        {

            string sql = "select * from humanresource.department";
            List<DeptListModel> dept = (List<DeptListModel>)QueryModule.DataSource<DeptListModel>(sql);


            foreach(DeptListModel obj in dept)
            {
                sql = "select * from humanresource.position where dept_id = '" + obj.dept_id + "'";
                obj.positions = (List<PositionModel>)QueryModule.DataSource<PositionModel>(sql);
            }


            sql = "select * from humanresource.employee_type";
            List<object> emp_type = (List<object>)QueryModule.DataSource<object>(sql);



            List<object> list = new List<object>();
            list.Add(new {department = dept, employeeType = emp_type });

            return list;
        }

        public List<object> GetList(int? status_id = 0)
        {
            string sql = @"select 
			emp.employee_id,
			emp.emp_account_no,
			emp.person_guid as `person_id`,
			emp.employee_name,
			`status`.status_name as `status`, 
			pos.position_desc as `position_name`, 
			dept.short_desc as `dept_name`
 
            from humanresource.employees emp

            inner join humanresource.employee_status stat on stat.employee_id = emp.employee_id and stat.end_time is NULL
            inner join humanresource.`status` `status` on `status`.status_id = stat.status_id
            inner join humanresource.person_position perpos on perpos.person_id = emp.person_guid and perpos.end_time is NULL
            inner join humanresource.position pos on pos.position_id = perpos.position_id
            inner join humanresource.person_dept perdept on perdept.person_id = emp.person_guid and perdept.end_time is NULL
            inner join humanresource.department dept on dept.dept_id = perdept.dept_id

            order BY emp.employee_name ASC";
            List<object> employees = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("employee");
            //SELECT
            //        emp_acc_no.emp_account_no as q,
            //        'an' as tag
            //        FROM humanresource.`employees` emp_acc_no
            //        UNION
            //UNION

            //        SELECT
            //        dept.short_desc as q,
            //        'd' as tag
            //        FROM humanresource.`employees` emp
            //        INNER JOIN humanresource.person_dept perdept on perdept.person_id = emp.person_guid and perdept.end_time is NULL
            //        INNER JOIN humanresource.department dept on dept.dept_id = perdept.dept_id
            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT emp.employee_name as q, 'en' as tag
                    FROM humanresource.`employees` emp
                    

                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();
            list.Add(employees);
            list.Add(form);
            list.Add(searches);

            return list;
        }

        public List<object> GetActiveList(int? status_id = 0)
        {
            string sql = @"select 
			emp.employee_id,
			emp.emp_account_no,
			emp.person_guid as `person_id`,
			emp.employee_name,
			`status`.status_name as `status`, 
			pos.position_desc as `position_name`, 
			dept.short_desc as `dept_name`
 
            from humanresource.employees emp

            inner join humanresource.employee_status stat on stat.employee_id = emp.employee_id and stat.end_time is NULL
            inner join humanresource.`status` `status` on `status`.status_id = stat.status_id
            inner join humanresource.person_position perpos on perpos.person_id = emp.person_guid and perpos.end_time is NULL
            inner join humanresource.position pos on pos.position_id = perpos.position_id
            inner join humanresource.person_dept perdept on perdept.person_id = emp.person_guid and perdept.end_time is NULL
            inner join humanresource.department dept on dept.dept_id = perdept.dept_id
            where `status`.status_name = 'Active'
            order BY emp.employee_name ASC";
            List<object> employees = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("employee");
            //SELECT
            //        emp_acc_no.emp_account_no as q,
            //        'an' as tag
            //        FROM humanresource.`employees` emp_acc_no
            //        UNION
            //UNION

            //        SELECT
            //        dept.short_desc as q,
            //        'd' as tag
            //        FROM humanresource.`employees` emp
            //        INNER JOIN humanresource.person_dept perdept on perdept.person_id = emp.person_guid and perdept.end_time is NULL
            //        INNER JOIN humanresource.department dept on dept.dept_id = perdept.dept_id
            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT emp.employee_name as q, 'en' as tag
                    FROM humanresource.`employees` emp
                    

                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();
            list.Add(employees);
            list.Add(form);
            list.Add(searches);

            return list;
        }

        public bool Insert(EmployeeModel model)
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
                    model.employee_id = Guid.NewGuid().ToString();
                    string sql = "insert into humanresource.employees (" + ObjectSqlMapping.MapInsert<EmployeeModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    sql = "insert into humanresource.employee_status (`employee_id`, `status_id`) VALUES (@id, @status_id)";
                    QueryModule.Execute<int>(sql, new {id = model.employee_id, status_id = 1 });


                    model.person_position.status = "Active";
                    sql = "insert into humanresource.person_position (" + ObjectSqlMapping.MapInsert<PersonPosition>() + ")";
                    QueryModule.Execute<int>(sql, model.person_position);


                    model.person_dept.status = "Active";
                    sql = "insert into humanresource.person_dept (" + ObjectSqlMapping.MapInsert<PersonDepartment>() + ")";
                    QueryModule.Execute<int>(sql, model.person_dept);


                    model.person_payroll_position.status = "Active";
                    sql = "insert into humanresource.person_payroll_position (" + ObjectSqlMapping.MapInsert<PersonPosition>() + ")";
                    QueryModule.Execute<int>(sql, model.person_payroll_position);


                    model.person_payroll_dept.status = "Active";
                    sql = "insert into humanresource.person_payroll_dept (" + ObjectSqlMapping.MapInsert<PersonDepartment>() + ")";
                    QueryModule.Execute<int>(sql, model.person_payroll_dept);


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
