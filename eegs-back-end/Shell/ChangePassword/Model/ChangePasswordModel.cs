using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Shell.ChangePassword.Model
{
    public class ChangePasswordModel
    {
        public string user_guid { get; set; }
        public string password { get; set; }
        public string confirm { get; set; }

    }
}
