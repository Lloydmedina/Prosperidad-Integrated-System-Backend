using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.HR.Position_Setup.Model
{
    public class PositionModel
    {
        public string position_id { get; set; }
        public string position_name { get; set; }
        public string position_desc { get; set; }
        public int position_type_id { get; set; }
        public int salary_grade { get; set; }
        //public string salary_grade { get; set; }
        //public string step { get; set; }
        public string dept_id { get; set; }
        public string status { get; set; }
    }
}
