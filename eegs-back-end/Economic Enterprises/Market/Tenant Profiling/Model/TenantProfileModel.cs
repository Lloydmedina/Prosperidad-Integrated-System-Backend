using eegs_back_end.Economic_Enterprises.Market.Property_Setup.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Economic_Enterprises.Market.Tenant_Profiling.Model
{
    public class TenantProfileModel
    {
        public string tenant_profile_id { get; set; }
        public string tenant_id { get; set; }
        public string tenant_name { get; set; }
        public string tenant_type { get; set; }
        public string tenant_contact { get; set; }
        public string tenant_address { get; set; }
        public string status { get; set; }
        public List<TenantProfileDetail> tenant_profile_dtl { get; set; }
        public TenantProfileStatus status_model { get; set; }
    }

    public class TenantProfileDetail
    {
        public string dtl_id { get; set; }
        public string main_id { get; set; }
        public DateTime date_start { get; set; }
        public DateTime date_end { get; set; }
        public DateTime transaction_date { get; set; }
        public string property_id { get; set; }
        public decimal rental_amount { get; set; }
        public string status { get; set; }
        public PropertyInfo propertyInfo { get; set; }
        public List<TenantProfileSub> sub_tenant { get; set; }
    }

    public class PropertyInfo : PropertyModel
    {
        public string section { get; set; }
    }

    public class TenantProfileSub
    {

        public string tenant_id { get; set; }
        public string tenant_name { get; set; }
        public string dtl_id { get; set; }
    }

    public class TenantProfileStatus
    {
        public string tenant_profile_id { get; set; }
        public string user_id { get; set; }
        public string activity { get; set; }
        public string remarks { get; set; }
        public string prev_record { get; set; }
        public int status_id { get; set; }
    }
}
