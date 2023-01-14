using eegs_back_end.DbModule;
using eegs_back_end.Shell.Form.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace eegs_back_end.Helpers
{
    public class Forms
    {

        public  static dynamic getForm(string path)
        {
          string  sql = "select form_guid from general.activity where executable_path LIKE '%"+ path +"%' limit 1";
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



            if(form.show_signatory == "true")
            {
                valuePairs.Add("signatories", getSignatories(form_id));
            }
          

            return (ExpandoObject)obz;

        }

        static object getSignatories(string form_id)
        {
            //GET SIGNATORIES
            string sql = @"SELECT 
			assign.assign_type_id, 
			assign.assign_type_name, 
			type.signatory_type_name,
			case when assign.assign_type_id = 1 then 
			(select CONCAT(person.first_name,' ', LEFT(UPPER(person.middle_name), 1), '. ', person.last_name) from general.users u
				inner join general.person person on person.person_guid = u.person_guid
				where u.user_guid = '" + GlobalObject.user_id + @"'
			) 
			when assign.assign_type_id = 3 THEN
			(	select 
				employee_name from humanresource.employees emp
				inner join humanresource.person_position ppos on ppos.person_id = emp.person_guid and ppos.end_time is NULL
				inner join humanresource.position pos on pos.position_id = ppos.position_id
				where pos.dept_id = main.dept_id and pos.role = 500
			)
			when assign.assign_type_id = 4 THEN
			(	select 
				employee_name from humanresource.employees emp
				inner join humanresource.person_position ppos on ppos.person_id = emp.person_guid and ppos.end_time is NULL
				inner join humanresource.position pos on pos.position_id = ppos.position_id
				where pos.role = 900
			)
            END
             as  signatory,
            case when assign.assign_type_id = 1 then 'Employee'
			when assign.assign_type_id = 3 THEN 'Department Head'
			when assign.assign_type_id = 4 THEN	'Municipal Mayor'
            END
            as position
            



            FROM general.`signatory_dtl` sigdtl

            inner join general.signatory_assign_type assign on assign.assign_type_id = sigdtl.assign_type_id
            inner join general.signatory_type type on type.signatory_type_id = sigdtl.signatory_type_id
            inner join general.signatory_main main on main.signatory_main_id = sigdtl.signatory_main_id

            inner join humanresource.department dept on dept.dept_id = main.dept_id

            where main.form_id = '" + form_id + "'";

            List<SigObj> sign = (List<SigObj>)QueryModule.DataSource<SigObj>(sql);

            var sigObj = new ExpandoObject() as IDictionary<string, object>;
            var sigObjList = new List<ExpandoObject>();
            foreach (SigObj ob in sign)
            {
                sigObj = new ExpandoObject() as IDictionary<string, object>;


                string propName = ob.signatory_type_name.Replace(" ", "_").ToLower();
                if (ob.signatory == null) ob.signatory = "";
                sigObj.Add(propName, ob.signatory);
                sigObj.Add("type",ob.signatory_type_name);
                sigObj.Add("position", ob.position);
                sigObjList.Add((ExpandoObject)sigObj);
            }

            return sign;
        }
    }

    public class SigObj
    {
        public string signatory_type_name { get; set; }
        public string signatory { get; set; }
        public string position { get; set; }
    }

    
}
