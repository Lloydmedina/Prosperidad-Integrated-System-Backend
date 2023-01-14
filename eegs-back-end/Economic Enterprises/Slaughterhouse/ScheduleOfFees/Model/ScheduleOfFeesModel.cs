using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Economic_Enterprises.Slaughterhouse.ScheduleOfFees.Model
{
    public class ScheduleOfFeesModel
    {
        public string schedule_of_fees_id { get; set; }
        public string receiving_id { get; set; }
        public DateTime date_slaughtered { get; set; }
        public string inspector_id { get; set; }
        public string inspector_name { get; set; }
        public string findings { get; set; }
        public string brand { get; set; }
        public string status { get; set; }
    }
}
