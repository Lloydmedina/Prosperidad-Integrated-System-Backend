using eegs_back_end.DbModule;
using eegs_back_end.Helpers;
using eegs_back_end.Shell.Domain.Model;
using eegs_back_end.Shell.Form.Model;
using eegs_back_end.Shell.Login.Model;
using eegs_back_end.Shell.Project.Model;
using eegs_back_end.Shell.RoleType.Model;
using eegs_back_end.Shell.User.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Shell.Login.Repository
{
    public interface ILoginRepository : IGlobalInterface
    {
        List<object> Login(LoginRequest credentials);

    }
    public class LoginRepository : ILoginRepository
    {
        public List<object> Login(LoginRequest credentials)
        {
            credentials.password = Cryptography.EncryptString(GlobalObject.key, credentials.password);
            string sql = @"SELECT general.users.*, domain.domain_name FROM general.`users`
                        INNER JOIN general.domain ON general.domain.domain_guid = general.users.domain_guid
                        WHERE general.users.UserName_User = @username AND general.users.Password_User = @password";
            UserModel user = (UserModel)QueryModule.DataObject<UserModel>(sql, credentials);

            if (user == null) return null;

            GlobalObject.user_id = user.user_guid;
            sql = @"SELECT general.project_title.* from general.project_title
                    inner join general.domain on general.domain.project_title_guid = general.project_title.project_title_guid
                    where general.domain.domain_guid = @domain_guid";
            ProjectModel project = (ProjectModel)QueryModule.DataObject<ProjectModel>(sql, user);

            sql = "select * from general_address.lgu_city_mun_config where project_title_guid = @id";
            project.lgu_city_mun_config = (LGUCityMunConfig)QueryModule.DataObject<LGUCityMunConfig>(sql, new { id = project.project_title_guid });


            sql = @"SELECT * FROM general.user_roletype
                    inner join general.roletype on general.roletype.roletype_guid = general.user_roletype.roletype_guid
                    WHERE general.user_roletype.user_guid = @user_guid";
            List<UserRoleType> userRoleTypes = (List<UserRoleType>)QueryModule.DataSource<UserRoleType>(sql, user);

            string roletype_guid = userRoleTypes.Select(x => x.roletype_guid).FirstOrDefault();

            sql = @"select general.roletype_activity.activity_guid from general.roletype_activity where general.roletype_activity.roletype_guid = '" + roletype_guid + "'";
            List<string> activities = (List<string>)QueryModule.DataSource<string>(sql);

            string activity_guids = ObjectSqlMapping.SetParam(activities.ToArray());

            sql = @"select general.activity.*, general.action_type.* from general.activity
                    inner join general.action_type on general.action_type.action_type_id = general.activity.action_type_id
                    where general.activity.activity_guid in (" + activity_guids + ")";
            List<Activity> activityTypes = (List<Activity>)QueryModule.DataSource<Activity>(sql);

            string form_guids = ObjectSqlMapping.SetParam(activityTypes.Select(x => x.form_guid).Distinct().ToArray());

            //sql = @"select distinct general.tbl_user_form.form_guid 
            //        from general.tbl_user_form 
            //        where general.tbl_user_form.parent_guid = ''
            //        AND general.tbl_user_form.domain_guid = @domain_guid";
            sql = @"select distinct general.tbl_user_form.parent_guid 
                    from general.tbl_user_form 
                    where  tbl_user_form.form_guid IN (" + form_guids + ")";
            List<string> parent_guid_list = (List<string>)QueryModule.DataSource<string>(sql, new {domain_guid = user.domain_guid });



            string parent_guids = ObjectSqlMapping.SetParam(parent_guid_list.ToArray());

            sql = @"select distinct general.tbl_user_form.parent_guid 
                    from general.tbl_user_form 
                    where  tbl_user_form.form_guid IN (" + parent_guids + ")";
            List<string> super_parent_guids = (List<string>)QueryModule.DataSource<string>(sql, new { domain_guid = user.domain_guid });

            string super_parent_guids_ = ObjectSqlMapping.SetParam(super_parent_guids.ToArray());

            string merged_guids = form_guids + ", " + parent_guids + ", " + super_parent_guids_;

            sql = "select * from general.form_sort where user_id = '" + GlobalObject.user_id + "'";
            List<object> seqList = (List<object>)QueryModule.DataSource<object>(sql);

            string join = "";
            string con = "";
            if (seqList.Count > 0)
            {
                join = "left join general.form_sort seq on seq.form_id = frm.form_guid and seq.user_id = '" + GlobalObject.user_id + @"' ";
                con = @" order by seq.`order` asc";
            }

            sql = @"select distinct frm.* from general.tbl_user_form frm
                    " + join + @"
                    where frm.form_guid in (" + merged_guids + @")
                    " + con + "";
            List<FormModel> forms = (List<FormModel>)QueryModule.DataSource<FormModel>(sql);
            




            List<int> removeIndexs = new List<int>();
            object[] Route = new object[activityTypes.Count];
            if (forms != null && forms.Count > 0)
            {
                int cnt = 0;
                int k = 0;
                foreach(var z in forms)
                {
                    if (z.parent_guid != null && z.parent_guid != "")
                    {
                        int index = forms.FindIndex(x => x.form_guid == z.parent_guid);
                        
                        if(index > -1)
                        {
                            if(forms[index].child == null)
                            {
                                forms[index].child = new List<FormModel>();
                            }

                            z.routes = activityTypes.AsEnumerable().Where(r => r.form_guid == z.form_guid).ToList();
                            if (z.routes.Count == 0)
                            {
                                z.routes = null;
                            }
                            else {
                                foreach (var x in z.routes)
                                {
                                    Route[k] = x.executable_path;
                                    k++;
                                }
                            }

                            forms[index].child.Add(z);

                            removeIndexs.Add(cnt);
                        }
                    }
                    cnt++;
                }

            }
            for (int i = removeIndexs.Count - 1; i >= 0; i--)
            {
                forms.RemoveAt(removeIndexs[i]);
            }

            // REMOVE node that dont have child
            for (int i = forms.Count - 1; i >= 0; i--)
            {
                if (forms[i].child == null && forms[i].parent_guid != "")
                {
                    forms.RemoveAt(i);
                }
            }

            List<object> list = new List<object>();

            list.Add(new { users = user });
            list.Add(new { project = project });
            list.Add(new { roletype = userRoleTypes });
            list.Add(new { forms = forms });
            list.Add(new { routes = Route});


            List<object> obj = new List<object>();

            obj.Add(new { Status = "Logged In" });

            return list;

        }
    }
}
