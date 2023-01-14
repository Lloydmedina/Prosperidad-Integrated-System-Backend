using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.Department.Model
{
    public class DepartmentModel
    {
        public string dept_id { get; set; }
        public string domain_id { get; set; }
        public string prefix { get; set; }
        public string dept_name { get; set; }
        public string short_desc { get; set; }
        public string dept_code { get; set; }
        public string type { get; set; }
        public DepartmentHead dept_head { get; set; }
        public string status { get; set; }
    }

    public class DepartmentHead
    {
        public string pk_id { get; set; }
        public string dept_id { get; set; }
        public string head_employee_id { get; set; }
        public string asst_employee_id { get; set; }
        public string status { get; set; }
    }
}
