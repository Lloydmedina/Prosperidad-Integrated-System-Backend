using eegs_back_end.DbModule;
using eegs_back_end.Shell.Form.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace eegs_back_end.Helpers
{
    [Route("form-config")]
    [ApiController]
    public class FormConfigController : ControllerBase
    {
        [HttpGet]
        public object Get([FromQuery] string path)
        {
            return getForm(path);
        }
        public static dynamic getForm(string path)
        {
            string sql = "select form_guid from general.activity where executable_path LIKE '%" + path + "%' limit 1";
            string form_id = (string)QueryModule.DataObject<string>(sql);


            sql = @"select * from general.tbl_user_form where tbl_user_form.form_guid = @form_guid";
            FormModel form = (FormModel)QueryModule.DataObject<FormModel>(sql, new { form_guid = form_id });

            dynamic obz = new ExpandoObject();

            IDictionary<string, object> valuePairs = obz;

            PropertyInfo[] formProperty;
            formProperty = form.GetType().GetProperties();


            foreach (PropertyInfo prop in formProperty)
            {
                valuePairs.Add(prop.Name, form.GetType().GetProperty(prop.Name).GetValue(form, null));
            }




            return (ExpandoObject)obz;

        }

        [HttpGet("mayor")]
        public object GetMayor()
        {
            string sql = @"select 
				employee_name, pos.position_name from humanresource.employees emp
				inner join humanresource.person_position ppos on ppos.person_id = emp.person_guid and ppos.end_time is NULL
				inner join humanresource.position pos on pos.position_id = ppos.position_id
				where pos.role = 900";
            List<object> list = (List<object>)QueryModule.DataSource<object>(sql);

            return list;
        }
    }
}
