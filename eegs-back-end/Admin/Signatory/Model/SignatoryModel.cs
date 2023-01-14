//using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.Signatory.Model
{
    public class SignatoryModel
    {
        public string signatory_main_id { get; set; }
        public string dept_id { get; set; }
        public string dept_name { get; set; }
        public string signatory_name { get; set; }
        public string status { get; set; }
        public string form_id { get; set; }
        public List<SignatoryDetail> signatoryDetails { get; set; }
    }

    public class SignatoryDetail
    {
        public string signatory_dtl_id { get; set; }
        public string signatory_main_id { get; set; }
        public int assign_type_id { get; set; }
        public int signatory_type_id { get; set; }
    }
}
