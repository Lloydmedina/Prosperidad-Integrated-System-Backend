using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.Elected_Officials.Model
{
    public class EOModel
    {
        public string eo_id { get; set; }
        public string mayor_id { get; set; }
        public string vmayor_id { get; set; }
        public string term_from { get; set; }
        public string term_to { get; set; }
        public string status { get; set; }

        public List<EOCouncillor> councillors { get; set; }
    }
    public class  EOCouncillor
    {
        public string eo_id { get; set; }
        public string councillor_id { get; set; }

    }

    public class EOMainPpl
    {
        public string mayor { get; set; }
        public string mayor_pos { get; set; }
        public string vmayor { get; set; }
        public string vmayor_pos { get; set; }
    }

    public class EOPrintCC
    {
        public string name { get; set; }
        public string title { get; set; }
    }
}
