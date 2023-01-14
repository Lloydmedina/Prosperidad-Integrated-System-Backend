using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Shell.RoleType.Model
{
    public class RoleTypeModel
    {
        public string rolename { get; set; }
        public string roletype_guid { get; set; }
        public string domain_guid { get; set; }

        public List<string> activity_guid { get; set; }
        public DateTime transaction_date { 
            get
            {
                return DateTime.UtcNow;
            }
            set { }
        }
        public string status { get; set; }
    }

    public class UserRoleType
    {
        public string user_guid { get; set; }
        public string roletype_guid { get; set; }
        public string rolename { get; set; }
        public bool isDefault { get; set; }
    }

    public class NodeKey
    {

        public string title { get; set; }
        public string key { get; set; }
        public string parent_guid { get; set; }
        public string domain_guid { get; set; }
        public string dept_id { get; set; }
        public string description { get; set; }
        public string[] activity { get; set; }
        public List<NodeKey> children { get; set; }
    }
}
