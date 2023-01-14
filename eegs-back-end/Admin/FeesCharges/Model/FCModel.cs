using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.FeesCharges.Model
{
    public class FCModel
    {
        public string fees_pk_id { get; set; }
        public string fees_code { get; set; }
        public string fees_name { get; set; }
        public string fees_desc { get; set; }
        public decimal initial_amount { get; set; }
        public string parent_id { get; set; }
        public string account_id { get; set; }
        public int fees_type_id { get; set; }
        public List<FCDeptForm> dept_form { get; set; }
        public List<FCRange> range_fees { get; set; }
        public string status { get; set; }
    }

    public class FCDeptForm
    {
        public string fees_pk_id { get; set; }
        public string dept_id { get; set; }
        public string form_id { get; set; }
    }

    public class FCRange
    {
        public string fees_pk_id { get; set; }
        public decimal amt_from { get; set; }
        public decimal amt_to { get; set; }
        public decimal fees { get; set; }

    }
}
