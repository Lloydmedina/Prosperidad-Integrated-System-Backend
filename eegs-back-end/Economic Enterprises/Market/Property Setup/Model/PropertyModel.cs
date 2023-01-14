using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Economic_Enterprises.Market.Property_Setup.Model
{
    public class PropertyModel
    {
        public string property_id { get; set; }
        public string property_name { get; set; }
        public string property_desc { get; set; }
        public int property_type_id { get; set; }
        public int property_brgy_id { get; set; }
        public int property_area { get; set; }
        public string property_bldg_id { get; set; }
        public string parent_id { get; set; }
        public List<PropertyFloor> property_floor { get; set; }
        public PropertyAmount property_amount { get; set; }
        public PropertyStatus property_status { get; set; }
    }




    public class PropertyFloor
    {
        public string property_id { get; set; }
        public string property_name { get; set; }
        public int property_type_id { get; set; }
        public int property_brgy_id { get; set; }
        public int property_area { get; set; }
        public string property_bldg_id { get; set; }
        public string parent_id { get; set; }
        public List<PropertySection> property_section { get; set; }
        public List<UnitStall> unit_stall { get; set; }
        public PropertyAmount property_amount { get; set; }
    }

    public class UnitStall
    {
        public string property_id { get; set; }
        public string property_name { get; set; }
        public int property_type_id { get; set; }
        public int property_brgy_id { get; set; }
        public int property_area { get; set; }
        public string property_bldg_id { get; set; }
        public string parent_id { get; set; }
        public PropertySection property_section { get; set; }
        public PropertyAmount property_amount { get; set; }
    }

        public class PropertySection
    {
        public string property_id { get; set; }
        public string section_id { get; set; }
    }

    public class PropertyAmount 
    { 
        public string property_id { get; set; }
        public string fees_pk_id { get; set; }
        public decimal initial_amount { get; set; }
    }

    public class PropertyStatus
    {
        public string property_id { get; set; }
        public int status_id { get; set; }
        public string user_id { get; set; }
        public string remarks { get; set; }
    }
}
