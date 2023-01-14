using System;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;

namespace eegs_back_end.GlobalHandler.Error.Response
{
    public class ErrorResponse
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }

        public ErrorResponse(Exception ex)
        {
            Type = ex.GetType().Name;
            Message = ex.Message;
            StackTrace = ex.ToString();
        }
    }
}
