using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Health.health_payment.Model

{

    public class HealthCardPayment
    {
        public string payment_id { get; set; }
        public string main_pk_id { get; set; }
        public string or_id { get; set; }
        public string or_date { get; set; }
        public decimal amount_paid { get; set; }
        public decimal total_fee { get; set; }
        public string payment_type { get; set; }
        public int payment_status { get; set; }
    }

}
