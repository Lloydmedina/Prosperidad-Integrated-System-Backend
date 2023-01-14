using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.EvacuationCenterSetup.Model
{
    public class EvacuationCenterModel
    {
        public string evacuation_center_guid { get; set; }
        public int region_id { get; set; }
        public int province_id { get; set; }
        public int citmun_id { get; set; }
        public int barangay_id { get; set; }
        public string street { get; set; }
        public string description { get; set; }
        public int capacity { get; set; }
        public string status { get; set; }
        public string form_trans_no { get; set; }
        public string venue_condition { get; set; }
        public string venue { get; set; }
        public string venue_status { get; set; }
        public int count { get; set; }
    }
}
