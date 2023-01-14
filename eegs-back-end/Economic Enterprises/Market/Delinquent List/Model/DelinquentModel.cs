using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Economic_Enterprises.Market.Delinquent_List.Model
{
    public class DelinquentModel
    {
       public string delinquent_list_id { get; set; }
        public DateTime date_generated { get; set; }
        public string record { get; set; }
        public string user_id { get; set; }
    }
}
