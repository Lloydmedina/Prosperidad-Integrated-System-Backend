using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.BusinessSetup.Model
{
    public class BusinessModel
    {
        public string form_trans_no { get; set; }
        public string business_id { get; set; }
        public string business_name { get; set; }
        public string trade_name { get; set; }
        public string owner_id { get; set; }
        public string owner_name { get; set; }
        public int reg_status { get; set; }
        public string mobile_no { get; set; }
        public string tel_no { get; set; }
        public string email { get; set; }
        public string street { get; set; }
        public string building { get; set; }
        public int brgy_id { get; set; }
        public int entity_id { get; set; }
        public BusinessStatus status { get; set; }
    }

    public class BusinessStatus
    {
        public string business_id { get; set; }
        public int status_id { get; set; }
        public string prev_record { get; set; }
        public string activity { get; set; }
        public string user_id { get; set; }
    }

    public class SearchSuggestionModel
    {
        public string business_name { get; set; }
        public string trade_name { get; set; }
        public string brgy_name { get; set; }
    }
}
