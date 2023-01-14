using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Economic_Enterprises.Slaughterhouse.Receiving.Model
{
    public class ReceivingModel
    {
        public string main_id { get; set; }
        public DateTime transaction_date { get; set; }
        public string status { get; set; }
        public  List<ReceivingObj> receiving_list { get; set; }


      
    }
    public class ReceivingObj
    {
        public DateTime time { get; set; }
        public string main_id { get; set; }
        public string receiving_id { get; set; }
        public string client_name { get; set; }
        public string client_id { get; set; }
        public string address { get; set; }
        public int animal_id { get; set; }
        public string no_of_heads { get; set; }
        public string kilos { get; set; }
        public DateTime slaughtering_date { get; set; }
        public DateTime slaughtering_time { get; set; }
        public string purpose { get; set; }
        public string lechonero { get; set; }
        public string status { get; set; }

        public AnimalInspection animal_inspection { get; set; }

    }

    public class AnimalInspection
    {
        public string receiving_id { get; set; }
        public string findings { get; set; }
        public string inspector_id { get; set; }
        public string inspector_name { get; set; }
        public string status { get; set; }
    }
}
