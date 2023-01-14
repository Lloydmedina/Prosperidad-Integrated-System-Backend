using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.HR.Employee_Setup.Model
{
    public class EmployeeModel
    {
        public string employee_id { get; set; }
        public int employee_type_id { get; set; }
        public string person_guid { get; set; }
        public string emp_account_no { get; set; }
        public string trans_date { get; set; }
        public string employee_name { get; set; }
        public string email_address { get; set; }
        public DateTime date_hired { get; set; }
        public string is_same_payroll { get; set; }
        public PersonPosition person_position { get; set; }
        public PersonDepartment person_dept { get; set; }
        public PersonPosition person_payroll_position { get; set; }
        public PersonDepartment person_payroll_dept { get; set; }
    }
    public class PersonPosition
    {
        public string person_id { get; set; }
        public string position_id { get; set; }
        public string status { get; set; }
    }
    public class PersonDepartment
    {
        public string person_id { get; set; }
        public string dept_id { get; set; }
        public string status { get; set; }
    }
}
