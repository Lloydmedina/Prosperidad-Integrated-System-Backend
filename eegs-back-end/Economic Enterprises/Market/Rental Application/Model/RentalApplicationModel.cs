using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Economic_Enterprises.Market.Rental_Application.Model
{
    public class RentalApplicationModel
    {
        public string form_trans_no { get; set; }
        public string rental_application_id { get; set; }
        public string applicant_id { get; set; }
        public string applicant_name { get; set; }
        public int application_type_id { get; set; }
        public string occupation_a { get; set; }
        public string nature_of_business { get; set; }
        public string address { get; set; }
        public string spouse { get; set; }
        public string occupation_b { get; set; }
        public string type_of_building { get; set; }
        public decimal expected_actual_capital { get; set; }
        public DateTime application_date { get; set; }
        public string status { get; set; }
        public List<RentalApplicationRequirements> reqs { get; set; }
        public RentalApplicationStatus rental_application_status { get; set; }
    }

    public class RentalApplicationRequirements
    {

        public string rental_application_id { get; set; }
        public int requirements_id { get; set; }
        public string submitted { get; set; }
        public string file { get; set; }
        public DateTime date_submitted { get; set; }
    }

    public class RentalApplicationStatus
    {
        public string rental_application_id { get; set; }
        public int status_id { get; set; }
        public string activity { get; set; }
        public string place_issued { get; set; }
        public string user_id { get; set; }
        public string remarks { get; set; }
    }
}
