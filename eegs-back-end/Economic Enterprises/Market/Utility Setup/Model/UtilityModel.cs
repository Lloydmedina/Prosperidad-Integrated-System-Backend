using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Economic_Enterprises.Market.Utility_Setup.Model
{
    public class UtilityModel
    {
        public string utility_id { get; set; }
        public string utility_type { get; set; }
        public string property_id { get; set; }
        public string property_name { get; set; }
        public decimal min { get; set; }
        public decimal max { get; set; }
        public decimal count { get; set; }
        public decimal step { get; set; }
        public string isChecked { get; set; }
        public string status { get; set; }
        public string remarks { get; set; }
        public List<Utility> utilities { get; set; }
        
    }
    public class Utility
    {
        public string utility_id { get; set; }
        public decimal min_value { get; set; }
        public decimal max_value { get; set; }
        public string rate_type { get; set; }
        public decimal rate { get; set; }
    }
}
