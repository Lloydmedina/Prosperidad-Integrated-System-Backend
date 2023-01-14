using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Shell.Domain.Model
{
    public class DomainModel
    {
        public string path { get; set; }
        public string form_trans_no { get; set; }
        public string domain_name { get; set; }
        public string description { get; set; }
        public string domain_guid { get; set; }
        public string project_title_guid { get; set; }
        public string route_reference { get; set; }
        public string status { get; set; }
        public DateTime transaction_date { 
            get { return DateTime.UtcNow; }
            set { }
        }
    }
}
