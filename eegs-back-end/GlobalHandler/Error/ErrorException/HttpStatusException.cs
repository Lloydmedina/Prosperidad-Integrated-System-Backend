using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace eegs_back_end.GlobalHandler.Error.ErrorException
{
    public class HttpStatusException : Exception
    {

        public HttpStatusCode Status { get; private set; }
        public HttpStatusException(HttpStatusCode status, string msg) : base(msg)
        {
            Status = status;
        }

    }
}
