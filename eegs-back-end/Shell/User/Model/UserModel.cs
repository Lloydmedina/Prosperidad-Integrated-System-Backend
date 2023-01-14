using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Shell.User.Model
{
    public class UserModel
    {
        public string domain_guid { get; set; }
        public List<string> roletype_guid { get; set; }
        public string UserFull_Name { get; set; }
        public string user_guid { get; set; }
        public string email_address { get; set; }
        public string person_guid { get; set; }
        public string employee_guid { get; set; }
        public string UserName_User { get; set; }
        public string Password_User { get; set; }
        public string status { get; set; }
        public string datesave { get; set; }
        public string domain_name { get; set; }
        //public string domain_name { get; set; }
    }
}
