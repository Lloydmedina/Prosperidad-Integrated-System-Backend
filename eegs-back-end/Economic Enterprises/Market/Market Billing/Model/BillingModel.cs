using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Economic_Enterprises.Market.Market_Billing.Model
{
    public class BillingModel
    {
        public string form_trans_no { get; set; }
        public string billing_id { get; set; }
        public string billing_no { get; set; }
        public string property_id { get; set; }
        public string tenant_profile_id { get; set; }
        public decimal bill_amount { get; set; }
        public DateTime billing_date { get; set; }
        public DateTime due_date { get; set; }
        public DateTime transaction_date { get; set; }
        public string status { get; set; }
        public BillingUtility billing_water { get; set; }
        public BillingUtility billing_electricity { get; set; }
        public BillingStatus billing_status { get; set; }

    }

    public class BillingUtility
    {
        public string billing_id { get; set; }
        public DateTime date_from { get; set; }
        public DateTime date_to { get; set; }
        public decimal prev_reading { get; set; }
        public decimal curr_reading { get; set; }
        public decimal consumption { get; set; }
        public decimal rate { get; set; }
        public string status { get; set; }
        
    }

    public class BillingStatus
    {

        public string billing_id { get; set; }
        public string user_id { get; set; }
        public string activity { get; set; }
        public string remarks { get; set; }
        public string prev_record { get; set; }
        public int status_id { get; set; }
    }
}
