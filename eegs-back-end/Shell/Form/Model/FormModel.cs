using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Shell.Form.Model
{
    public class FormModel
    {

        public string form_guid { get; set; }
        public string form_name { get; set; }
        public string form_alias { get; set; }
        public string form_number { get; set; }
        public string form_status { get; set; }
        public string parent_guid { get; set; }
        public int series_length { get; set; }
        public int series_start { get; set; }
        public string series_reset { get; set; }
        public string series_ref_no { get; set; }
        public string series_separator { get; set; }
        public string domain_guid { get; set; }
        public string form_type { get; set; }
        public int default_filter_id { get; set; }
        public string form_icon { get; set; }
        public string domain_path { get; set; }
        public string with_series { get; set; }
        public string with_fees { get; set; }
        public string allow_print { get; set; }
        public string print_option { get; set; }
        public string show_header { get; set; }
        public string show_footer { get; set; }
        public string show_signatory { get; set; }
        public List<Activity> routes { get; set; }
        public List<FormModel> child { get; set; }
        public List<FormSeries> form_series { get; set; }
    }
    public class FormSeries
    {
        public string form_guid { get; set; }
        public string series_include { get; set; }
        public string series_format { get; set; }
        public int series_order { get; set; }
        public string series_type { get; set; }
    }
    public class Activity
    {
        public string activity_guid { get; set; }
        public string activity_name { get; set; }
        public string description { get; set; }
        public string action_type_id { get; set; }
        public string form_guid { get; set; }
        public string executable_path { get; set; }
    }
}
