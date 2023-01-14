using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Accounting.AccountTitle.Model
{
    public class AccountModel
    {
        public string account_code { get; set; }
        public string account_name { get; set; }
        public string account_desc { get; set; }
        public int parent_id { get; set; }
        public decimal initial_amount { get; set; }
        public int account_type_id { get; set; }
        public int activity_type_id { get; set; }
        public string status { get; set; }
    }
}
