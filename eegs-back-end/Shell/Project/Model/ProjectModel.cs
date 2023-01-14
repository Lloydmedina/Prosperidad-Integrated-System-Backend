using eegs_back_end.Shell.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Shell.Project.Model
{
    public class ProjectModel
    {
        public string project_title_guid { get; set; }
        public string title_name { get; set; }
        public string tin { get; set; }
        public string sss { get; set; }
        public string pagibig { get; set; }
        public string philhealth { get; set; }
        public string description { get; set; }
        public string tel_no { get; set; }
        public string mobile_no { get; set; }
        public string email_address { get; set; }
        public string website { get; set; }
        public string status { get; set; }

        public LGUCityMunConfig lgu_city_mun_config { get; set; }

        public LGUReportConfiguration report_config { get; set; }
    }
    
    public class LGUCityMunConfig
    {
        public string project_title_guid { get; set; }
        public string municipal_code { get; set; }
        public int province_id { get; set; }
        public int zipcode_id { get; set; }
        public int city_mun_id { get; set; }
        public string city_mun_name { get; set; }
        
        public string status_name { get; set; }
        public string province_area { get; set; }
        public string zip_code { get; set; }
        public string region { get; set; }
        public int region_id { get; set; }
        public string street { get; set; }
        public string barangay { get; set; }
        public int barangay_id { get; set; }
    }

    public class LGUReportConfiguration
    {

        public string project_title_guid { get; set; }
        public string header1 { get; set; }
        public string header2 { get; set; }
        public string header3 { get; set; }
        public string footer1 { get; set; }
        public string footer2 { get; set; }
        public string footer3 { get; set; }
        public string header_logo1 { get; set; }
        public string header_logo2 { get; set; }
        public string base_64_1 { get; set; }
        public string base_64_2 { get; set; }
        public string file_name1 { get; set; }
        public string file_name2 { get; set; }
        public string footer_align { get; set; }
    }
}
